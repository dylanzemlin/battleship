static const float maxFloat = 3.402823466e+38;

float2 RaySphere(float3 sphereCentre, float sphereRadius, float3 rayOrigin, float3 rayDir) {
    float3 offset = rayOrigin - sphereCentre;
    float a = 1; // Set to dot(rayDir, rayDir) if rayDir might not be normalized
    float b = 2 * dot(offset, rayDir);
    float c = dot (offset, offset) - sphereRadius * sphereRadius;
    float d = b * b - 4 * a * c; // Discriminant from quadratic formula

    // Number of intersections: 0 when d < 0; 1 when d = 0; 2 when d > 0
    if (d > 0) {
        float s = sqrt(d);
        float dstToSphereNear = max(0, (- b - s) / (2 * a));
        float dstToSphereFar = (- b + s) / (2 * a);

        // Ignore intersections that occur behind the ray
        if (dstToSphereFar >= 0) {
            return float2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
        }
    }
    // Ray did not intersect sphere
    return float2(maxFloat, 0);
}

float4 triplanar(float3 vertPos, float3 normal, float scale, UnityTexture2D tex) {

	// Calculate triplanar coordinates
	float2 uvX = vertPos.zy * scale;
	float2 uvY = vertPos.xz * scale;
	float2 uvZ = vertPos.xy * scale;

	float4 colX = tex2D (tex, uvX);
	float4 colY = tex2D (tex, uvY);
	float4 colZ = tex2D (tex, uvZ);
	// Square normal to make all values positive + increase blend sharpness
	float3 blendWeight = normal * normal;
	// Divide blend weight by the sum of its components. This will make x + y + z = 1
	blendWeight /= dot(blendWeight, 1);
	return colX * blendWeight.x + colY * blendWeight.y + colZ * blendWeight.z;
}

float4 triplanarOffset(float3 vertPos, float3 normal, float3 scale, UnityTexture2D tex, float2 offset) {
	float3 scaledPos = vertPos / scale;
	float4 colX = tex2D (tex, scaledPos.zy + offset);
	float4 colY = tex2D(tex, scaledPos.xz + offset);
	float4 colZ = tex2D (tex,scaledPos.xy + offset);
	
	// Square normal to make all values positive + increase blend sharpness
	float3 blendWeight = normal * normal;
	// Divide blend weight by the sum of its components. This will make x + y + z = 1
	blendWeight /= dot(blendWeight, 1);
	return colX * blendWeight.x + colY * blendWeight.y + colZ * blendWeight.z;
}

float3 ObjectToTangentVector(float4 tangent, float3 normal, float3 objectSpaceVector) {
	float3 normalizedTangent = normalize(tangent.xyz);
	float3 binormal = cross(normal, normalizedTangent) * tangent.w;
	float3x3 rot = float3x3 (normalizedTangent, binormal, normal);
	return mul(rot, objectSpaceVector);
}

// Reoriented Normal Mapping
// http://blog.selfshadow.com/publications/blending-in-detail/
// Altered to take normals (-1 to 1 ranges) rather than unsigned normal maps (0 to 1 ranges)
float3 blend_rnm(float3 n1, float3 n2)
{
	n1.z += 1;
	n2.xy = -n2.xy;

	return n1 * dot(n1, n2) / n1.z - n2;
}

// Sample normal map with triplanar coordinates
// Returned normal will be in obj/world space (depending whether pos/normal are given in obj or world space)
// Based on: medium.com/@bgolus/normal-mapping-for-a-triplanar-shader-10bf39dca05a
float3 triplanarNormal(float3 vertPos, float3 normal, float3 scale, float2 offset, UnityTexture2D normalMap) {
	float3 absNormal = abs(normal);

	// Calculate triplanar blend
	float3 blendWeight = saturate(pow(normal, 4));
	// Divide blend weight by the sum of its components. This will make x + y + z = 1
	blendWeight /= dot(blendWeight, 1);

	// Calculate triplanar coordinates
	float2 uvX = vertPos.zy * scale + offset;
	float2 uvY = vertPos.xz * scale + offset;
	float2 uvZ = vertPos.xy * scale + offset;

	// Sample tangent space normal maps
	// UnpackNormal puts values in range [-1, 1] (and accounts for DXT5nm compression)
	float3 tangentNormalX = UnpackNormal(tex2D(normalMap, uvX));
	float3 tangentNormalY = UnpackNormal(tex2D(normalMap, uvY));
	float3 tangentNormalZ = UnpackNormal(tex2D(normalMap, uvZ));

	// Swizzle normals to match tangent space and apply reoriented normal mapping blend
	tangentNormalX = blend_rnm(half3(normal.zy, absNormal.x), tangentNormalX);
	tangentNormalY = blend_rnm(half3(normal.xz, absNormal.y), tangentNormalY);
	tangentNormalZ = blend_rnm(half3(normal.xy, absNormal.z), tangentNormalZ);

	// Apply input normal sign to tangent space Z
	float3 axisSign = sign(normal);
	tangentNormalX.z *= axisSign.x;
	tangentNormalY.z *= axisSign.y;
	tangentNormalZ.z *= axisSign.z;

	// Swizzle tangent normals to match input normal and blend together
	float3 outputNormal = normalize(
		tangentNormalX.zyx * blendWeight.x +
		tangentNormalY.xzy * blendWeight.y +
		tangentNormalZ.xyz * blendWeight.z
	);

	return outputNormal;
}

float3 triplanarNormalTangentSpace(float3 vertPos, float3 normal, float3 scale, float4 tangent, UnityTexture2D normalMap) {
	float3 textureNormal = triplanarNormal(vertPos, normal, scale, 0, normalMap);
	return ObjectToTangentVector(tangent, normal, textureNormal);
}

float3 triplanarNormalTangentSpace(float3 vertPos, float3 normal, float3 scale, float2 offset, float4 tangent, UnityTexture2D normalMap) {
	float3 textureNormal = triplanarNormal(vertPos, normal, scale, offset, normalMap);
	return ObjectToTangentVector(tangent, normal, textureNormal);
}

void WaterEffect_float(
    in float depth,
    in float4 color,
    in float3 view_dir,
    in float3 view_vec,
    in float3 cam_pos,
    in float3 main_light_dir,
    in float3 oceanCenter,
    in float oceanRadius,
    in float depthMult,
    in float alphaMult,
    in float4 colorA,
    in float4 colorB,
    in float smoothness,
    in float planetScale,

    // Waves
    // in SamplerState waveSampler,
    in UnitySamplerState sampl,
    in UnityTexture2D waveATexture,
    in UnityTexture2D waveBTexture,
    in float waveStrength,
    in float waveNormalScale,
    in float waveSpeed,

    out float4 result
)
{
    float2 hit = RaySphere(oceanCenter, oceanRadius, cam_pos, view_dir);
    float toOcean = hit.x;
    float throughOcean = hit.y;
    float oceanViewDepth = min(throughOcean, depth - toOcean);

    // direction to main light
    float3 dir_to_sun = -main_light_dir;
    float3 rayOceanIntersectPos = cam_pos + view_dir * toOcean - oceanCenter;

    if (oceanViewDepth > 0)
    {
        float opticalDepth = 1 - exp(-oceanViewDepth * depthMult / planetScale);
        float alpha = 1 - exp(-oceanViewDepth * alphaMult / planetScale);
        float4 oceanColor = lerp(colorA, colorB, opticalDepth);
        float oceanSphereNormal = normalize(rayOceanIntersectPos);

        // Waves
        float2 waveOffsetA = float2(_Time.x * waveSpeed, _Time.x * waveSpeed * 0.8);
        float2 waveOffsetB = float2(_Time.x * waveSpeed * -0.8, _Time.x * waveSpeed * -0.3);

        float3 waveNormal = triplanarNormal(rayOceanIntersectPos, oceanSphereNormal, waveNormalScale / planetScale, waveOffsetA, waveATexture);
        waveNormal = triplanarNormal(rayOceanIntersectPos, waveNormal, waveNormalScale / planetScale, waveOffsetB, waveBTexture);
        waveNormal = normalize(lerp(oceanSphereNormal, waveNormal, waveStrength));

        float diffuse_lighting = saturate(dot(oceanSphereNormal, dir_to_sun));
        float specularAngle = acos(dot(normalize(view_vec), waveNormal));
        float specularEx = specularAngle / (1 - smoothness);
        float specularHighlight = exp(-specularEx * specularEx);

        oceanColor *= diffuse_lighting;
        oceanColor += specularHighlight;

        result = lerp(color, oceanColor, alpha);
        // result = 1;
        return;
    }

    result = color;
}
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    public ParticleSystem[] particleSystems; // Array of particle systems to control

    public void Move(Vector3 position)
    {
        transform.position = position;
    }

    public void SetForward(Vector3 forward)
    {
        transform.forward = forward;
    }

    public void Rotate(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    public void Play()
    {
        foreach (var system in particleSystems)
        {
            if (system != null && !system.isPlaying)
            {
                system.Play();
            }
        }
    }

    public void Stop()
    {
        foreach (var system in particleSystems)
        {
            if (system != null && system.isPlaying)
            {
                system.Stop();
            }
        }
    }
}

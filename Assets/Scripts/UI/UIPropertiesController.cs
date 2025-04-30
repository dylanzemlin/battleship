using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPropertiesController : MonoBehaviour
{
    [Header("Planet")]
    public Planet planet;

    [Header("Noise Image")]
    public Image noiseImage;

    [Header("Planet Properties")]
    public Slider noiseOffsetSlider;
    public TMP_Text noiseOffsetText;

    public Slider continentThresholdSlider;
    public TMP_Text continentThresholdText;

    public Slider octavesSlider;
    public TMP_Text octavesText;

    public Slider lacunaritySlider;
    public TMP_Text lacunarityText;

    public Slider persistenceSlider;
    public TMP_Text persistenceText;

    public Slider baseFrequencySlider;
    public TMP_Text baseFrequencyText;

    // initial values
    private float initialNoiseOffset;
    private float initialContinentThreshold;
    private int initialOctaves;
    private float initialLacunarity;
    private float initialPersistence;
    private float initialBaseFrequency;

    private void InitializeProperties()
    {
        noiseOffsetSlider.value = planet.terrainOptions.noiseOffset;
        noiseOffsetText.text = $"Noise Offset: {planet.terrainOptions.noiseOffset:F2}";
        noiseOffsetSlider.onValueChanged.AddListener(OnNoiseOffsetSliderValueChanged);

        continentThresholdSlider.value = planet.terrainOptions.continentThreshold;
        continentThresholdText.text = $"Continent Threshold: {planet.terrainOptions.continentThreshold:F2}";
        continentThresholdSlider.onValueChanged.AddListener(OnContinentThresholdSliderValueChanged);

        octavesSlider.value = planet.terrainOptions.octaves;
        octavesText.text = $"Octaves: {planet.terrainOptions.octaves}";
        octavesSlider.onValueChanged.AddListener(OnOctavesSliderValueChanged);

        lacunaritySlider.value = planet.terrainOptions.lacunarity;
        lacunarityText.text = $"Lacunarity: {planet.terrainOptions.lacunarity:F2}";
        lacunaritySlider.onValueChanged.AddListener(OnLacunaritySliderValueChanged);

        persistenceSlider.value = planet.terrainOptions.persistence;
        persistenceText.text = $"Persistence: {planet.terrainOptions.persistence:F2}";
        persistenceSlider.onValueChanged.AddListener(OnPersistenceSliderValueChanged);

        baseFrequencySlider.value = planet.terrainOptions.baseFrequency;
        baseFrequencyText.text = $"Base Frequency: {planet.terrainOptions.baseFrequency:F2}";
        baseFrequencySlider.onValueChanged.AddListener(OnBaseFrequencySliderValueChanged);

        // Store initial values
        initialNoiseOffset = planet.terrainOptions.noiseOffset;
        initialContinentThreshold = planet.terrainOptions.continentThreshold;
        initialOctaves = planet.terrainOptions.octaves;
        initialLacunarity = planet.terrainOptions.lacunarity;
        initialPersistence = planet.terrainOptions.persistence;
        initialBaseFrequency = planet.terrainOptions.baseFrequency;
    }

    private void UpdateNoiseImage()
    {
        if (planet == null || noiseImage == null)
        {
            return;
        }

        // create and assign the sprite as a texture
        Texture2D img = NoiseTextureGenerator.GenerateNoiseTexture(
            400,
            400,
            planet.terrainOptions
        );
        noiseImage.sprite = Sprite.Create(
            img,
            new Rect(0, 0, img.width, img.height),
            new Vector2(0.5f, 0.5f),
            100.0f
        );
        noiseImage.SetNativeSize();
        noiseImage.preserveAspect = true;
    }

    void Update()
    {
        if (planet == null)
        {
            planet = FindFirstObjectByType<Planet>();
            if (planet == null)
            {
                return;
            }

            // When the planet is found, initialize the properties
            InitializeProperties();
            UpdateNoiseImage();
        }

        // If the backspace key is pressed, reset the sliders to their initial values
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            noiseOffsetSlider.value = initialNoiseOffset;
            continentThresholdSlider.value = initialContinentThreshold;
            octavesSlider.value = initialOctaves;
            lacunaritySlider.value = initialLacunarity;
            persistenceSlider.value = initialPersistence;
            baseFrequencySlider.value = initialBaseFrequency;

            planet.Regenerate();
            UpdateNoiseImage();
        }
    }

    private void OnNoiseOffsetSliderValueChanged(float value)
    {
        planet.terrainOptions.noiseOffset = value;
        noiseOffsetText.text = $"Noise Offset: {value:F2}";
        planet.Regenerate();

        UpdateNoiseImage();
    }

    private void OnContinentThresholdSliderValueChanged(float value)
    {
        planet.terrainOptions.continentThreshold = value;
        continentThresholdText.text = $"Continent Threshold: {value:F2}";
        planet.Regenerate();

        UpdateNoiseImage();
    }

    private void OnOctavesSliderValueChanged(float value)
    {
        planet.terrainOptions.octaves = (int)value;
        octavesText.text = $"Octaves: {(int)value}";
        planet.Regenerate();

        UpdateNoiseImage();
    }

    private void OnLacunaritySliderValueChanged(float value)
    {
        planet.terrainOptions.lacunarity = value;
        lacunarityText.text = $"Lacunarity: {value:F2}";
        planet.Regenerate();

        UpdateNoiseImage();
    }

    private void OnPersistenceSliderValueChanged(float value)
    {
        planet.terrainOptions.persistence = value;
        persistenceText.text = $"Persistence: {value:F2}";
        planet.Regenerate();

        UpdateNoiseImage();
    }

    private void OnBaseFrequencySliderValueChanged(float value)
    {
        planet.terrainOptions.baseFrequency = value;
        baseFrequencyText.text = $"Base Frequency: {value:F2}";
        planet.Regenerate();

        UpdateNoiseImage();
    }
}

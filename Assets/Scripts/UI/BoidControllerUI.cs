using UnityEngine;
using UnityEngine.UI;

public class BoidControllerUI : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider separationCoeffSlider;
    public Slider alignmentCoeffSlider;
    public Slider cohesionCoeffSlider;

    [Header("Default Values")]
    public float defaultCoeffSeparation = 1.0f;
    public float defaultCoeffAlignment = 0.05f;
    public float defaultCoeffCohesion = 0.01f;

    void Start()
    {
        InitializeSliders();
    }

    void InitializeSliders()
    {
        if (separationCoeffSlider != null)
        {
            separationCoeffSlider.value = defaultCoeffSeparation;
            separationCoeffSlider.onValueChanged.AddListener(delegate { UpdateBoidParameters(); });
        }

        if (alignmentCoeffSlider != null)
        {
            alignmentCoeffSlider.value = defaultCoeffAlignment;
            alignmentCoeffSlider.onValueChanged.AddListener(delegate { UpdateBoidParameters(); });
        }

        if (cohesionCoeffSlider != null)
        {
            cohesionCoeffSlider.value = defaultCoeffCohesion;
            cohesionCoeffSlider.onValueChanged.AddListener(delegate { UpdateBoidParameters(); });
        }
    }

    public void UpdateBoidParameters()
    {
        Boid[] boids = FindObjectsByType<Boid>(FindObjectsSortMode.None);
        
        foreach (Boid boid in boids)
        {
            if (separationCoeffSlider != null) boid.CoeffSeparation = separationCoeffSlider.value;
            if (alignmentCoeffSlider != null) boid.CoeffAlignment = alignmentCoeffSlider.value;
            if (cohesionCoeffSlider != null) boid.CoeffCohesion = cohesionCoeffSlider.value;
        }
    }
}

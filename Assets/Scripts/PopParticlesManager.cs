using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopParticlesManager : MonoBehaviour {

    public List<GameObject> confettisPresets;
    public List<Material> confettiMat;

	// Use this for initialization
	void Start () {
        var loadedObjects = Resources.LoadAll("Prefabs/Confetti", typeof(GameObject));
        var loadedMaterials = Resources.LoadAll("Materials/Confetti", typeof(Material));
        confettisPresets.Clear();
        confettiMat.Clear();

        foreach (var loadedObject in loadedObjects)
        {
            confettisPresets.Add(loadedObject as GameObject);
        }
        foreach (var loadedMaterial in loadedMaterials)
        {
            confettiMat.Add(loadedMaterial as Material);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public GameObject RandomConfetti()
    {
        GameObject chosenCofetti = confettisPresets[Random.Range(0, confettisPresets.Count)];
        return chosenCofetti;
    }

    public Material RandomMat()
    {
        Material chosenMat = confettiMat[Random.Range(0, confettiMat.Count)];
        return chosenMat;
    }
}

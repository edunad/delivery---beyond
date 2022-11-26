
using System;
using TMPro;
using UnityEngine;

using Random = UnityEngine.Random;

public class entity_box_pickup_spot : MonoBehaviour {
    [Header("Settings")]
    public TextMeshPro text_1;
    public TextMeshPro text_2;
    public entity_box box;

    public void setID(int id) {
        this.text_1.text = "#" + id;
        this.text_2.text = "#" + id;

        this.box.ID = id;

        Array sizes = Enum.GetValues(typeof(BoxSize));
        this.box.setSize((BoxSize)sizes.GetValue(Random.Range(3, sizes.Length))); // Only small boxes :P
    }
}

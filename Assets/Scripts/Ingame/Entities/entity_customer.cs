
using System.Collections.Generic;
using UnityEngine;

public enum RequestType {
    SEND_ITEM = 0,
    WANT_ITEM
}

public class entity_customer : MonoBehaviour {
    public RequestType type;

    private List<string> _names = new List<string>() {
        "Reptifur",
        "Kooda",
        "D3lta",
        "Gathilo",
        "Thomas Wake",
        "Goose",
        "Gnu Phelps",
        "Kaleb Bamantis",
        "Coyote",
        "Bryson Biraccoon",
        "Augustin Blanco",
        "Justin Tuwolf",
        "John Bowen",
        "John Swann",
        "Mash",
        "Risa",
        "Zari",
        "Tish",
        "Kama",
        "Lith",
        "Viro",
        "Kosh",
        "Lal",
        "Zazi"
    };

    public void init() {
        this.name = this._names[Random.Range(0, this._names.Count)];
    }
}

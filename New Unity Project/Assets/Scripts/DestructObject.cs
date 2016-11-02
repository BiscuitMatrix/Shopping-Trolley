using UnityEngine;
using System.Collections;

public class DestructObject : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log(other.name);
        Destroy(gameObject); // When triggered object destroys itself
    }
}

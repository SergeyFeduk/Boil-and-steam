using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [field: SerializeField] public PlayerController Controller { get; private set; }
    [field: SerializeField] public PlayerInteractor interactor { get; private set; }
    public static Player inst { get; private set; }
    private void Awake() {

        if (inst != null && inst != this) {
            Destroy(this);
        } else {
            inst = this;
        }
    }
    
}
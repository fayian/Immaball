using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour {

    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private float fuel = 100.0f;

    //getters
    public float Speed() {
        return speed;
    }
    public float Fuel() {
        return fuel;
    }

    //methods
    public void SpeedUp(float amount) {
        if (amount < 0) Debug.LogWarning("SpeedUp getting a negetive number");
        speed += amount;
    }
    public void SpeedDown(float amount) {
        if (amount < 0) Debug.LogWarning("SpeedDown getting a negetive number");
        speed -= amount;
        speed = Mathf.Max(speed, 0);
    }
}

using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings inst { get; private set; }
    public GlobalSettingsSO main;

    private void Awake() {

        if (inst != null && inst != this) {
            Destroy(this);
        } else {
            inst = this;
        }
    }
}

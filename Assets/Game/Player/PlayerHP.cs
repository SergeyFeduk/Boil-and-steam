using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour//, PDamagable
{
    [SerializeField] private float HP, maxHP;
    [SerializeField] private Slider HPSlider;

    public void Damage(int damage) {
        if (HP - damage <= 0)
        {
            HP = 0;
            Death();
        }
        else if (HP - damage >= maxHP) //Можно наносить игроку отрицательный урон, тогда хил
        {
            HP = maxHP;
        }
        else
        {
            HP -= damage;
        }
        HPSlider.value = HP / maxHP;
    }

    public GameObject GetObject() {
        return Player.inst.gameObject;
    }


    private void Start() {
        //GetComponent<PDamagable>().Init();
    }
    public void Death()
    {
        print("Я умер, прости");
        //GetComponent<PDamagable>().PDestroy();
    }
}

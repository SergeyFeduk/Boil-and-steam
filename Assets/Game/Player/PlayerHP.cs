using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private float HP, maxHP;
    [SerializeField] private Slider HPSlider;

    public void Damage(int damage) {
        if (HP - damage <= 0)
        {
            HP = 0;
            Death();
        }
        else if (HP - damage >= maxHP) //����� �������� ������ ������������� ����, ����� ���
        {
            HP = maxHP;
        }
        else
        {
            HP -= damage;
        }
        HPSlider.value = HP / maxHP;
    }
    public void Death()
    {
        print("� ����, ������");
    }
}

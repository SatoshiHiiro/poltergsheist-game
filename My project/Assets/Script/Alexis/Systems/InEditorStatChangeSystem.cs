using UnityEngine;

[ExecuteInEditMode]
public class InEditorStatChangeSystem : MonoBehaviour
{
    PossessionController posCon;
    Rigidbody2D rb2D;

    [SerializeField] int lastEnum;
    [SerializeField] float[] customStats = new float[5];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*posCon = this.GetComponent<PossessionController>();
        rb2D = this.GetComponent<Rigidbody2D>();
        lastEnum = (int)posCon.baseStats;
        if (lastEnum == 0)
            ChangeCustomStats();

        SetStats(lastEnum);*/
    }

    // Update is called once per frame
    void Update()
    {
        /*int idEnum = (int)posCon.baseStats;
        if (lastEnum != idEnum)
        {
            if (lastEnum == 0)
                ChangeCustomStats();

            SetStats(idEnum);
            lastEnum = idEnum;
        }*/
    }

    void ChangeCustomStats()
    {
        customStats[0] = posCon.speed;
        customStats[1] = posCon.maxSpeed;
        customStats[2] = posCon.stopSpeed;
        customStats[3] = posCon.jumpSpeed;
        customStats[4] = rb2D.mass;
    }

    void SetStats(int idEnum)
    {
        switch (idEnum)
        {
            case 0: //Custom
                posCon.speed = customStats[0];
                posCon.maxSpeed = customStats[1];
                posCon.stopSpeed = customStats[2];
                posCon.jumpSpeed = customStats[3];
                rb2D.mass = customStats[4];
                break;

            case 1: //Lightest
                posCon.speed = 5f;
                posCon.maxSpeed = 5f;
                posCon.stopSpeed = 2f;
                posCon.jumpSpeed = 1.9f;
                rb2D.mass = .25f;
                break;

            case 2: //Light
                posCon.speed = 2;
                posCon.maxSpeed = 3.2f;
                posCon.stopSpeed = 2;
                posCon.jumpSpeed = 2;
                rb2D.mass = 5f;
                break;

            case 3: //Medium
                posCon.speed = 3;
                posCon.maxSpeed = 2.4f;
                posCon.stopSpeed = 3;
                posCon.jumpSpeed = 3;
                rb2D.mass = 10;
                break;

            case 4: //Heavy
                posCon.speed = 4;
                posCon.maxSpeed = 1.8f;
                posCon.stopSpeed = 4;
                posCon.jumpSpeed = 4;
                rb2D.mass = 20;
                break;

            case 5: //Heaviest
                posCon.speed = 5;
                posCon.maxSpeed = 1f;
                posCon.stopSpeed = 5;
                posCon.jumpSpeed = 5;
                rb2D.mass = 40;
                break;

            default:
                posCon.speed = 0;
                posCon.maxSpeed = 0;
                posCon.stopSpeed = 0;
                posCon.jumpSpeed = 0;
                rb2D.mass = 1;
                break;
        }
    }
}

public enum objecType
{
    /*Custom = 0,
    Lightest = 1,
    Light = 2,
    Medium = 3,
    Heavy = 4,
    Heaviest = 5*/

    Lightest = 6,
    Light = 5,
    Medium = 4,
    Heavy = 3,
    Heaviest = 1
}

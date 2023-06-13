using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class WorldItemThrower : MonoBehaviour
{
    [SerializeField] private TextAsset asset;
    [SerializeField] private Sprite shadowSprite;
    [SerializeField] private Vector2 deltaVector;

    [SerializeField] private float radius;
    [SerializeField, Range(0, 1f)] private float minRadiusPercent;
    [SerializeField] private float height;
    [SerializeField] private float startHeight;
    [SerializeField] private float flyingTime;
    [SerializeField] private float scale;

    [SerializeField] private AnimationCurve speedCurve;

    private EntitySerializer serializer = new EntitySerializer();
    private Vector2 startPosition;


    public static WorldItemThrower inst;
    private void Awake()
    {
        if (inst != null && inst != this)
        {
            Destroy(this);
        }
        else
        {
            inst = this;
        }
    }
    public void ThrowItems(Item items)
    { 
        Entity entity = serializer.Deserialize(asset.text) as Entity;

        entity.GetComponent<Renderable>().sprite = items.data.icon;
        entity.GetComponent<CWorldItem>().itemData = items.data;

        startPosition = ScreenUtils.WorldMouse() + deltaVector;
        if (items.count != 1) WaitAndThrow(entity, items.count);
        else
        {
            entity = World.inst.entityManager.InstantiateEntity(entity, false);
            entity.position = startPosition;
            entity.GetComponent<Renderable>().visualSize = Vector2.one * scale;
        }
    }
    private async void WaitAndThrow(Entity entity, int count)
    {
        for (int i = 0; i < count; i++)
        {
            entity.AddComponent(typeof(CIndependentRenderable));
            CIndependentRenderable iRen = entity.GetComponent<CIndependentRenderable>();
            iRen.ySorted = false;
            iRen.renderOrder = 1000000;
            iRen.sprite = shadowSprite;
            iRen.localPosition = new Vector2(0,0);
            entity = World.inst.entityManager.InstantiateEntity(entity, false);
            entity.position = startPosition;
            entity.GetComponent<Renderable>().visualSize = Vector2.one * scale;
            FlyItem(entity);
            if(i % 2 == 1) await Task.Yield();
        }
    }
    private async void FlyItem(Entity entity)
    {
        float angle = Random.Range(0f, 2f) * Mathf.PI;
        Vector2 startPoint = entity.position + new Vector2(0,startHeight); //без начальной высоты = поз для тети
        Vector2 endPoint = entity.position + new Vector2(Mathf.Cos(angle),Mathf.Sin(angle)) * Random.Range(minRadiusPercent,1f);
        Vector2 hPoint = (entity.position + endPoint) / 2 + new Vector2(0, height);
        Parabola parabola = new Parabola(entity.position, hPoint, endPoint);

        Timer timer = new Timer();
        timer.SetFrequency(1/flyingTime);
        float startTime = Time.time;
        entity.position = startPoint;
        while (!timer.Execute())
        {
            float x = Vector2.Lerp(startPoint, endPoint, (speedCurve.Evaluate(timer.GetTimePassed() / flyingTime))).x;
            entity.position = new Vector2(x, parabola.Calculate(x));
            await Task.Yield();
        }
        entity.position = endPoint;
        entity.GetComponent<CWorldItem>().readyToMove = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(ScreenUtils.WorldMouse(), radius);
        Vector3 to = new Vector3(ScreenUtils.WorldMouse().x, ScreenUtils.WorldMouse().y, -10);
        Gizmos.DrawLine(to, to + new Vector3(0, height));
    }
}

public class Parabola
{
    private float a, b, c;
    public Parabola(Vector2 A, Vector2 B, Vector2 C)
    {
        float x1 = A.x;
        float x2 = B.x;
        float x3 = C.x;
        float y1 = A.y;
        float y2 = B.y;
        float y3 = C.y;
        float q = y3 - ((x3 * (y2 - y1) + x2 * y1 - x1 * y2) / (x2 - x1));
        float w = x3 * (x3 - x1 - x2) + x1*x2;
        a = q/w;
        b = ((y2 - y1) / (x2 - x1)) - a * (x1 + x2);
        c = ((x2 * y1 - y2 * x1) / (x2 - x1)) + a * x1 * x2;
    }
    public float Calculate(float x)
    {
        return a * x * x + b * x + c;
    }
}

using System.Collections;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private SpriteRenderer[] _spriteRenderers = null;
    private Transform _hourHand = null;
    private Transform _minuteHand = null;

    private void OnEnable()
    {
        if (_spriteRenderers == null)
        {
            _spriteRenderers = new SpriteRenderer[3];
            _spriteRenderers[0] = GetComponent<SpriteRenderer>();
            _spriteRenderers[1] = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _spriteRenderers[2] = transform.GetChild(1).GetComponent<SpriteRenderer>();
        }
        if (_hourHand == null) _hourHand = Util.FindChild<Transform>(gameObject, "Hour", true);
        if (_minuteHand == null) _minuteHand = Util.FindChild<Transform>(gameObject, "Min", true);

        Timeback();
    }

    void Update()
    {
        _hourHand.transform.Rotate(Vector3.forward * 5);
        _minuteHand.transform.Rotate(Vector3.forward * 20);

        transform.LookAt(Camera.main.transform);
    }

    private void Timeback()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        yield return Util.GetWaitForSeconds(.75f);

        Color color0 = _spriteRenderers[0].color;
        Color color1 = _spriteRenderers[1].color;
        Color color2 = _spriteRenderers[2].color;

        while (true)
        {
            if (color0.a <= 0)
                break;

            color0.a -= .025f;
            color1.a -= .025f;
            color2.a -= .025f;

            _spriteRenderers[0].color = color0;
            _spriteRenderers[1].color = color1;
            _spriteRenderers[2].color = color2;

            yield return Util.GetWaitForSeconds(.01f);
        }
    }
}

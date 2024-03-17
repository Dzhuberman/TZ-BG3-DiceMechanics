using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Modifier : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txtTitle;
    [SerializeField] private TextMeshProUGUI _txtValue;

    [SerializeField] private GameObject _valueGameObject;

    public int Value { get; set; }

    public void Setup(string title, int value)
    {
        _txtTitle.text = title;
        Value = value;
        _txtValue.text = Value.ToString();
    }

    //Animation created using DOTween.
    //This method copy modifier's value and move it to result window.
    //After that it deletes the copy.
    public void PlayAnimation(Vector2 to, float duration)
    {
        GameObject value = Instantiate(_valueGameObject, _valueGameObject.transform.position, _valueGameObject.transform.rotation);
        value.transform.SetParent(FindObjectOfType<Canvas>().transform);
        value.transform.localScale = Vector3.one;

        value.GetComponent<RectTransform>().DOAnchorPos(to, duration);
        StartCoroutine(DeleteCopy(value, duration));
    }

    private IEnumerator DeleteCopy(GameObject g, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(g);
    }
}

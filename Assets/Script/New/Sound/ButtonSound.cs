using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Tambahkan namespace ini

public class ButtonSound : MonoBehaviour, IPointerEnterHandler
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(() => SoundManager.Instance.PlaySound2D("Click"));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Memainkan suara saat hover
        SoundManager.Instance.PlaySound2D("Hover");
    }
}
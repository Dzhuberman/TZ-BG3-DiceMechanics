using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject _lockPanel;
    [SerializeField] private Image _lockStateImage;

    private void Awake()
    {
        Instance = this;
    }

    public void LockInputs(bool lockState)
    {
        if (lockState)
        {
            _lockPanel.SetActive(true);
            InputManager.InputSystem.Disable();

            _lockStateImage.color = Color.red;
        }
        else
        {
            _lockPanel.SetActive(false);
            InputManager.InputSystem.Enable();
            
            _lockStateImage.color = Color.green;
        }
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}

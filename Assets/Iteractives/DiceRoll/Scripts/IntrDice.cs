using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class IntrDice : MonoBehaviour
{
    //Temporary setings for modifiers.
    [Header("Temporary For Testing")]
    [SerializeField] private bool _isCheater = false;
    [SerializeField] private Slider _numberToBeat;
    [SerializeField] private Slider _mainStat;
    [SerializeField] private Slider _additionalBonus;
    [SerializeField] private Toggle _d4Bonus;

    [Header("TMP")]
    [SerializeField] private TextMeshProUGUI _txtNumberToBeat;
    [SerializeField] private TextMeshProUGUI _txtResult;
    [SerializeField] private TextMeshProUGUI _txtEnd;

    [Header("Game Objects")]
    [SerializeField] private GameObject _txtResultGameObject;
    [SerializeField] private GameObject _bonusFrameGameObject;
    [SerializeField] private GameObject _modifierSlotsGameObject;
    [SerializeField] private GameObject _clkCloseGameObject;
    [SerializeField] private GameObject _endWindowGameObject;

    [Header("Params")]
    [Range(1, 20)]
    [SerializeField] private int _startingNumber = 5;

    [Header("Modifiers")]
    [SerializeField] private GameObject _blankModifierPrefab;
    [SerializeField] private List<GameObject> _modifiers;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _bonusSfx;

    [Header("End Screen Params")]
    [SerializeField] private Color _colorWin;
    [SerializeField] private Color _colorLose;
    [SerializeField] private string _txtWin;
    [SerializeField] private string _txtLose;

    private Animator _animator;
    private AudioSource _audioSource;

    private int _result;
    private bool _hasWon;

    [Header("Events")]
    public UnityEvent OnOpen;
    public UnityEvent OnWin;
    public UnityEvent OnLose;

    //Initialization. Called in "OnOpen" event.
    public void Init()
    {
        TestingSetup();

        _txtNumberToBeat.text = _startingNumber.ToString();

        _bonusFrameGameObject.SetActive(_modifiers.Count > 0);
    }

    //Setting up modifiers. If you want to turn it off, uncheck "Is Cheater" box in inspector.
    private void TestingSetup()
    {
        if (!_isCheater) return;

        _startingNumber = (int)_numberToBeat.value;

        if (_mainStat.value > 0) 
        {
            _modifiers.Add(Instantiate(_blankModifierPrefab, _modifierSlotsGameObject.transform));
            _modifiers[_modifiers.Count - 1].GetComponent<Modifier>().Setup("Main Stat", (int)(_mainStat.value - 10) / 2);
        }
        if (_additionalBonus.value > 0) 
        {
            _modifiers.Add(Instantiate(_blankModifierPrefab, _modifierSlotsGameObject.transform));
            _modifiers[_modifiers.Count - 1].GetComponent<Modifier>().Setup("Bonus", (int)_additionalBonus.value);
        }
        if (_d4Bonus.isOn)
        {
            _modifiers.Add(Instantiate(_blankModifierPrefab, _modifierSlotsGameObject.transform));
            _modifiers[_modifiers.Count - 1].GetComponent<Modifier>().Setup("1+D4", Random.Range(1, 5));
        }
    }

    private void OnEnable()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        OnOpen?.Invoke();
    }

    //Generating random number from 1 to 20. Depending on number sets animator parameter.
    //After main roll animation will play correlating end animation.
    //This method called in dice image click event.
    public void RollDice()
    {
        _result = Random.Range(1, 21);
        _animator.SetInteger("end_roll_number", _result);

        _animator.Play("dice_roll");

        ShowResult();
    }

    private void ShowResult()
    {
        _txtResult.text = _result.ToString();         
        _txtResultGameObject.SetActive(true);
    }

    //Coroutine that calculating and animating bonus addition to main result.
    private IEnumerator AnimateResult(float animDuration)
    {
        GameManager.Instance.LockInputs(true);

        for (int i = 0; i < _modifiers.Count; i++)
        {
            Modifier mod = _modifiers[i].GetComponent<Modifier>();

            mod.PlayAnimation(_txtResultGameObject.transform.position, animDuration);

            yield return new WaitForSeconds(animDuration);

            PlaySound(_bonusSfx);

            _result += mod.Value;
            _txtResult.text = _result.ToString();
        }

        yield return new WaitForSeconds(0.3f);
        _animator.SetTrigger("end_bonus_anim");

        _hasWon = _result > _startingNumber;
        _animator.SetBool("has_won", _hasWon);

        _clkCloseGameObject.SetActive(true);

    }

    //Called in "result_open" animation event.
    public void CalculateResult()
    {
        StartCoroutine(AnimateResult(0.8f));
    }

    private void PlaySound(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void ShowEndWindow()
    {
        if (_hasWon) 
        {
            _txtEnd.text = _txtWin;
            _txtEnd.color = _colorWin;
        }
        else
        {
            _txtEnd.text = _txtLose;
            _txtEnd.color = _colorLose;
        }

        _endWindowGameObject.SetActive(true);
    }

    //Resetting interactive for reuse purpose.
    public void IntrClose()
    {
        Reset();

        gameObject.SetActive(false);
        GameManager.Instance.LockInputs(false);

        if (_hasWon) 
        {
            OnWin?.Invoke();
        }
        else
        {
            OnLose?.Invoke();
        }
    }

    private void Reset()
    {
        _txtResultGameObject.SetActive(false);
        RectTransform rect1 = _txtResultGameObject.GetComponent<RectTransform>();
        rect1.localScale = new Vector3(0, 0, 1);

        _endWindowGameObject.SetActive(false);
        RectTransform rect2 = _endWindowGameObject.GetComponent<RectTransform>();
        rect2.localScale = new Vector3(0, 0, 1);

        foreach (Transform child in _modifierSlotsGameObject.transform) {
            Destroy(child.gameObject);
        }

        _modifiers.Clear();
    }
}

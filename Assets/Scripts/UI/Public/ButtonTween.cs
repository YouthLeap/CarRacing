using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTween : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 onUpScale; //keeps track of original scale

    /// <summary>
    /// What it should scsale to onDown event
    /// </summary>
    public Vector3 onDownScale = new Vector3(1.1f, 1.1f, 1.1f);

    /// <summary>
    /// How long the tween (scaling) should last in seconds. If set to 0 no tween is used, happens instantly.
    /// </summary>
    public float tweenDuration = .1f; 

    /// <summary>
    /// If button can be held down (will not be scale back to original until up/release)
    /// Can not be toggled at run-time
    /// </summary>
    public bool canButtonBeHeldDown = true; //can not be toggled at runtime

    private bool internalTweenInProgress = false;
    private Vector3 tweenTargetScale = Vector3.one;
    private Vector3 tweenStartingScale = Vector3.one;
    private float tweenTimeElapsed = 0;
	private float deltaTime = 1.0f / 60.0f;

	private Button button;

    void Awake()
    {
        onUpScale = transform.localScale;

		button = transform.GetComponent<Button> ();
    }

    void OnEnable()
    {
        internalTweenInProgress = false;
        tweenTimeElapsed = 0;
        transform.localScale = onUpScale;
    }

    void OnDisable()
    {

    }

	public void OnPointerDown(PointerEventData eventData)
    {
		if (button.interactable == false)
			return;

        if (tweenDuration <= 0)
        {
            transform.localScale = onDownScale;
        }
        else
        {
            transform.localScale = onUpScale;

            tweenTargetScale = onDownScale;
            tweenStartingScale = transform.localScale;
            if (!internalTweenInProgress)
            {
                StartCoroutine(ScaleTween());
                internalTweenInProgress = true;
            }
        }
    }

	public void OnPointerUp(PointerEventData eventData) 
    {
		if (button.interactable == false)
			return;

        if (tweenDuration <= 0)
        {
            transform.localScale = onUpScale;
        }
        else
        {
            tweenTargetScale = onUpScale;
            tweenStartingScale = transform.localScale;
            if (!internalTweenInProgress)
            {
                StartCoroutine(ScaleTween());
                internalTweenInProgress = true;
            }      
        }
    }

    private IEnumerator ScaleTween()
    {
        tweenTimeElapsed = 0;
        while (tweenTimeElapsed < tweenDuration)
        {
            transform.localScale = Vector3.Lerp(tweenStartingScale,tweenTargetScale,tweenTimeElapsed / tweenDuration);
            yield return null;
            tweenTimeElapsed += deltaTime;
        }
        transform.localScale = tweenTargetScale;
        internalTweenInProgress = false;

        //if button is not held down bounce it back
        if (!canButtonBeHeldDown)
        {
            if (tweenDuration <= 0)
            {
                transform.localScale = onUpScale;
            }
            else
            {
                tweenTargetScale = onUpScale;
                tweenStartingScale = transform.localScale;
                StartCoroutine(ScaleTween());
                internalTweenInProgress = true;
            }
        }
    }
}
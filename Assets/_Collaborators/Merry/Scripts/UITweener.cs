using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//code inspired by https://www.youtube.com/watch?app=desktop&v=Ll3yujn9GVQ&ab_channel=GameDevGuide

public enum UIAnimationTypes
{
    Move,
    Scale,
    ScaleX,
    ScaleY,
    Fade
}

public enum TweenState
{
  Showing, Hiding
}

public class UITweener : MonoBehaviour
{
    public GameObject objectToAnimate;
    public UIAnimationTypes animationType;
    public LeanTweenType easeType;
    public float duration, delay;

    public bool loop, pingPong;

    public bool startPositionOffset;
    public Vector3 from, to;

    private LTDescr _tweenObject;
    public bool _disabling;

    public bool fadeOnEnable;

    public TweenState tweenState;

    public void Start()
    {
        _disabling = false;
    }

    public void OnEnable()
    {
        if (_disabling == false)
        {
            if (fadeOnEnable)
            {
                Show();
            }
        }
    }

    public void Show()
    {
      SetDirection(TweenState.Showing);
        HandleTween();
    }

    public void HandleTween()
    {
        if (objectToAnimate == null)
        {
            objectToAnimate = gameObject;
        }

        switch (animationType)
        {
            case UIAnimationTypes.Fade:
                Fade();
                break;
            case UIAnimationTypes.Move:
                MoveAbsolute();
                break;
            case UIAnimationTypes.Scale:
                Scale();
                break;
            case UIAnimationTypes.ScaleX:
                Scale();
                break;
            case UIAnimationTypes.ScaleY:
                Scale();
                break;
        }

        _tweenObject.setDelay(delay);
        _tweenObject.setEase(easeType);

        if (loop)
        {
            _tweenObject.loopCount = int.MaxValue;
        }
        if (pingPong)
        {
            _tweenObject.setLoopPingPong();
        }
    }

    public void Fade()
    {
        if (gameObject.GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }

        if (startPositionOffset)
        {
            objectToAnimate.GetComponent<CanvasGroup>().alpha = from.x;
        }

        _tweenObject = LeanTween.alphaCanvas(objectToAnimate.GetComponent<CanvasGroup>(), to.x, duration);
    }

    public void MoveAbsolute()
    {
      // print(tweenState.ToString());
      if (tweenState == TweenState.Showing)
      {
        // print("Showing!");
          objectToAnimate.GetComponent<RectTransform>().anchoredPosition = from;
          _tweenObject = LeanTween.move(objectToAnimate.GetComponent<RectTransform>(), to, duration);
      }
      if (tweenState == TweenState.Hiding)
      {
        //  print("Hiding!");
          objectToAnimate.GetComponent<RectTransform>().anchoredPosition = to;
          _tweenObject = LeanTween.move(objectToAnimate.GetComponent<RectTransform>(), from, duration);
      }
    }

    public void Scale()
    {
        if (startPositionOffset)
        {
            objectToAnimate.GetComponent<RectTransform>().localScale = from;
        }
        _tweenObject = LeanTween.scale(objectToAnimate, to, duration);
    }


    public void SetDirection(TweenState direction)
    {
        tweenState = direction;
        // var temp = from;
        // from = to;
        // to = temp;
    }


    public void Hide()
    {   
        SetDirection(TweenState.Hiding);
        HandleTween();
        _tweenObject.setOnComplete(() =>
        {
         
        });
    }


}

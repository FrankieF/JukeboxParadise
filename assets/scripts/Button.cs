using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour
{
    public float colorTime, time, duration;
    public Color currentColor, idleColor, hoverColor, pressedColor;

    public UIWidget widget;

    public enum ButtonStates {Idle, Hover, Press};
    public ButtonStates currentState = ButtonStates.Idle;
    public ButtonStates targetState = ButtonStates.Idle;
    public void SetState(ButtonStates state)
    {
        targetState = state;
    }

    void Update()
    {
        if (targetState != currentState)
        {
            StopCoroutine(currentState.ToString());
            currentState = targetState;
            StartCoroutine(currentState.ToString());
        }
    }

    IEnumerator Idle()
    {
        currentColor = widget.color;
        time = 0;

        while (true)
        {
            time += Time.deltaTime;
            colorTime = time / duration;
            
            widget.color = Color.Lerp(currentColor, idleColor, colorTime);

            yield return null;
        }
    }

    IEnumerator Hover()
    {
        currentColor = widget.color;
        time = 0;

        while (true)
        {
            time += Time.deltaTime;
            colorTime = time / duration;

            widget.color = Color.Lerp(currentColor, hoverColor, colorTime);

            yield return null;
        }
    }

    IEnumerator Press()
    {
        currentColor = widget.color;
        time = 0;

        while (true)
        {
            time += Time.deltaTime;
            colorTime = time / duration;

            widget.color = Color.Lerp(currentColor, pressedColor, colorTime);

            yield return null;
        }
    }
}

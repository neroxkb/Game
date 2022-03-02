using UnityEngine;

using System.Collections;

using UnityEngine.UI;

using UnityEngine.EventSystems;

public class ButtonText: Button

{

    public Text text;

    protected override void DoStateTransition(SelectionState state, bool instant)

    {

        base.DoStateTransition(state, instant);

        switch (state)

        {

            case SelectionState.Disabled:
                this.transform.GetChild(0).gameObject.SetActive(false);
                break;

            case SelectionState.Highlighted:

                this.transform.GetChild(0).gameObject.SetActive(true);
                //text.gameObject.SetActive(true);

                break;

            case SelectionState.Normal:
                this.transform.GetChild(0).gameObject.SetActive(false);

                break;

            case SelectionState.Pressed:
                this.transform.GetChild(0).gameObject.SetActive(false);
                break;

            default:
                this.transform.GetChild(0).gameObject.SetActive(false);
                break;

        }

    }

}
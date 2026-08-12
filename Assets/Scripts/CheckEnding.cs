using UnityEngine;

public class CheckEnding : MonoBehaviour
{
    public bool ending1 = false;
    public bool ending2 = false;
    public bool ending3 = false;
    public bool ending4 = false;
    public bool ending5 = false;

    private void Start()
    {
        if (ending1)
        {
            PlayerStatus.Instance.ending1Achieved = true;
            return;
        }
        else if (ending2)
        {
            PlayerStatus.Instance.ending2Achieved = true;
            return;
        }
        else if (ending3)
        {
            PlayerStatus.Instance.ending3Achieved = true;
            return;
        }
        else if (ending4)
        {
            PlayerStatus.Instance.ending4Achieved = true;
            return;
        }
        else if (ending5)
        {
            PlayerStatus.Instance.ending5Achieved = true;
            return;
        }
    }
}

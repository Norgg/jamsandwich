using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carriable : MonoBehaviour {

    private bool carried = false;
    public bool IsBeingCarried()
    {
        return carried;
    }

    public void SetCarry(bool carryVal)
    {
        carried = carryVal;
    }
}

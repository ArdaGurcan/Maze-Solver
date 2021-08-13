using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour
{
	private void OnMouseDown()
	{
        Destroy(gameObject);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivableMachine
{
	bool IsActive{ get; }

	void Activate ();
}

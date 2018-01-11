using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSMB : StateMachineBehaviour {

	public float damping = 0.15f;

	private readonly int horizontalParaHash = Animator.StringToHash("Horizontal");
	private readonly int verticalParaHash = Animator.StringToHash("Vertical");
	private readonly int jumpParaHash = Animator.StringToHash("Jump");
	private readonly int grabParaHash = Animator.StringToHash("Grab");

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		animator.SetFloat(horizontalParaHash, horizontal, damping, Time.deltaTime);
		animator.SetFloat(verticalParaHash, vertical, damping, Time.deltaTime);

		if(Input.GetKeyDown("e")) {
			animator.SetTrigger(grabParaHash);
		}
		if(Input.GetKeyDown("space")) {
			animator.SetTrigger(jumpParaHash);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floater : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float depthBeforeSubmerged = 1f;
    public float displacemetAmount = 3f;
    public int floatercount = 1;
    public float waterdrag = 0.99f;
    public float waterangularDrag = 0.5f;


    private void FixedUpdate()
    {
        rigidBody.AddForceAtPosition(Physics.gravity / floatercount, transform.position, ForceMode.Acceleration);

        float waveheight = WaveManager.instance.GetWaveHeight(transform.position.x);
        if (transform.position.y < waveheight)
        {
            float displacementMultiplier = Mathf.Clamp01((waveheight - transform.position.y) / depthBeforeSubmerged) * displacemetAmount;
            rigidBody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
            rigidBody.AddForce(displacementMultiplier * -rigidBody.velocity * waterdrag * Time.deltaTime, ForceMode.VelocityChange);
            rigidBody.AddTorque(displacementMultiplier * -rigidBody.angularVelocity * waterangularDrag * Time.deltaTime, ForceMode.VelocityChange);
        }
    }
}

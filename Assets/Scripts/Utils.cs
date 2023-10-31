using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public static class Utils
{
    public static Vector3 GetMouseWorldPosition(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return worldPos;
    }

    // Enables Aiming for weapons held by the player, points at mouse pos
    public static void EnableAiming(GameObject weapon)
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector3 aimDir = (mousePos - weapon.transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        if (angle > -90f && angle < 90f)
            weapon.GetComponent<SpriteRenderer>().flipY = false;
        else
            weapon.GetComponent<SpriteRenderer>().flipY = true;
        weapon.transform.eulerAngles = new Vector3(0, 0, angle);
    }
    public static Vector2 RandomVector2(float angle, float angleMin)
    {
        float random = Random.value * angle + angleMin;
        return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
    }
    public static Vector2 RotateVector2(Vector2 vector, float angleDegrees)
    {
        // Convert the angle to radians because Mathf.Cos and Mathf.Sin use radians
        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        // Calculate the new rotated vector
        float x = vector.x * Mathf.Cos(angleRadians) - vector.y * Mathf.Sin(angleRadians);
        float y = vector.x * Mathf.Sin(angleRadians) + vector.y * Mathf.Cos(angleRadians);

        return new Vector2(x, y);
    }
    public static void AnimationState(Animator mAnimator, string currState)
    {
        if (mAnimator != null)
        {
            foreach (AnimatorControllerParameter parameter in mAnimator.parameters)
            {
                mAnimator.SetBool(parameter.name, false);
            }
            mAnimator.SetBool(currState, true);
        }
    }
    public static void FlipSprites(GameObject mObj, GameObject player) 
    {
        mObj.GetComponent<SpriteRenderer>().flipX = (player.transform.position.x <= mObj.transform.position.x);
    }
    public static float PlayerDistance(GameObject mObj, GameObject player) 
    {
        return Vector3.Distance(mObj.transform.position, player.transform.position); 
    }
    public static bool isPlayerClose(GameObject me, GameObject player, float range)
    {
        if (player == null || me == null)
            return false;

        float distance = Vector3.Distance(me.transform.position, player.transform.position);
        if (distance >= range)
            return false;
        else
            return true;
    }
    public static void DrawLine(LineRenderer line, GameObject me, GameObject at)
    {
        line.enabled = true;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.material.color = Color.red;
        line.SetPosition(0, me.transform.position);
        line.SetPosition(1, at.transform.position);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
//´ÅÌúÐ«½Å±¾
public class MagnctScorpionSripts : MonsterObject
{
    private Direction currentDirection;
    protected override void Ability()
    {
        PlayerController.instance.enableRequest++;
        StartCoroutine(SubController());
    }
    IEnumerator SubController()
    {
        while (true)
        {
            yield return null;
            if (Input.GetButtonDown("w_key"))
            {
                ChooseDirection(Direction.Up);
            }
            else if (Input.GetButtonDown("a_key"))
            {
                ChooseDirection(Direction.Left);
            }
            else if (Input.GetButtonDown("s_key"))
            {
                ChooseDirection(Direction.Down);
            }
            else if (Input.GetButtonDown("d_key"))
            {
                ChooseDirection(Direction.Right);
            }
            if (Input.GetButtonUp("skill"))
            {
                End();
                UseSkill();
                yield break;
            }
        }
    }
    private void UseSkill()
    {
        Vector3 vector3 = transform.position + currentDirection.ToVector3();
        RaycastHit2D[] hits = Physics2D.RaycastAll(vector3, Vector2.zero);
        if (hits != null && hits.Length != 0)
        {
            foreach (var item in hits)
            {
                IronBoxSripts gameSripts;
                item.collider.gameObject.TryGetComponent<IronBoxSripts>(out gameSripts);
                if (gameSripts != null)
                {
                    StartCoroutine(KeepMove(gameSripts, 10, currentDirection));
                    return;
                }
            }
        }
        for (int i = 1; i < 9; i++)
        {
            Vector3 vector31 = vector3 + currentDirection.ToVector3() * i;
            if (BoxMoveCheck(vector31, currentDirection) != null)
                return;
        }
    }
    IEnumerator KeepMove(IronBoxSripts ironBoxSripts, int i, Direction direction)
    {
        ConsumeAbilityValue();
        for (int j = 0; j < i; j++)
        {
            ReturnState returnState = ironBoxSripts.CheckMove(direction);
            if (returnState.canMove == true)
            {
                foreach (var box in returnState.boxObjects)
                {
                    box.Move(direction);
                }
            }

            else
                break;
            yield return new WaitForSeconds(ironBoxSripts.moveSpeed);
        }
    }
    private IronBoxSripts BoxMoveCheck(Vector3 vector3, Direction direction)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(vector3, Vector2.zero);
        if (hits != null && hits.Length != 0)
        {
            foreach (var item in hits)
            {
                IronBoxSripts gameSripts;
                item.collider.gameObject.TryGetComponent<IronBoxSripts>(out gameSripts);
                if (gameSripts != null)
                {
                    StartCoroutine(KeepMove(gameSripts, 10, direction.Rotate(2)));
                    return gameSripts;
                }
            }
        }
        return null;
    }
    private void ChooseDirection(Direction direction)
    {
        Turn(direction);
        currentDirection = direction;
    }
    protected override void SetAttributes()
    {
        _size = Size.medium;
        _dietType = DietType.Carnivore;
        _habitat = Habitat.Terrestrial;
    }
}

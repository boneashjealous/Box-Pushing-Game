using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdieSripts : MonsterObject
{
    //缓冲的动画控制器
    private RuntimeAnimatorController animator0;
    private Sprite sprite0;
    protected override void Awake()
    {
        base.Awake();
        animator0 = Resources.Load<RuntimeAnimatorController>("Animations/Monster/Birdie/Birdie_0");
        sprite0 = Resources.LoadAll<Sprite>("Sprite/GameSprite/HitWallBirdie_0")[0];
    }

    //切换飞行与着陆
    protected override void Ability()
    {
        if (_habitat == Habitat.Aerial)
        {
            _habitat = Habitat.Terrestrial;
            spriteRenderer.sprite = sprite;
        }
        else
        {
            _habitat = Habitat.Aerial;
            spriteRenderer.sprite = sprite0;
        }
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        RuntimeAnimatorController animator1 = animator.runtimeAnimatorController;
        animator.runtimeAnimatorController = animator0;
        animator0 = animator1;
        ConsumeAbilityValue();
    }

    protected override void SetAttributes()
    {
        _size= Size.small;
        _dietType= DietType.Herbivore;
        _habitat= Habitat.Terrestrial;
    }
}

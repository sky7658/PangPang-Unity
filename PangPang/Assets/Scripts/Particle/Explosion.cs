using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PangPang.Particle
{
    public class Explosion : MonoBehaviour
    {
        private string _name;
        private bool isAnimEnd;
        private Animator anim;
        private void Awake()
        {
            anim = GetComponent<Animator>();
        }
        public void Execute(int x, int y, string name)
        {
            this._name = name;
            transform.position = BaseInfo.SetBlockPos(y, x, 0);
            isAnimEnd = true;

            StartCoroutine(ExplosionAction());
        }

        private void Update()
        {
            isAnimEnd = (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) ? false : true;
        }
        private IEnumerator ExplosionAction()
        {
            while(isAnimEnd) yield return null;

            ExplosionPool.instance.ReturnObject(gameObject, _name);

            yield break;
        }
    }

}

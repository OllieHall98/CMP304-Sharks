using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FLS;
using FLS.Rules;
using FLS.MembershipFunctions;

namespace BoatChase
{
    public class FuzzyChaseState : FuzzyBaseState
    {
        protected IFuzzyEngine engine_chaseSpeed;
        protected IFuzzyEngine engine_playerEscape;

        private LinguisticVariable distanceToPlayer;
        private LinguisticVariable chaseSpeed;
        private LinguisticVariable lostPlayer;

        public FuzzyChaseState(Monster pawn) : base(pawn)
        {
            engine_chaseSpeed = new FuzzyEngineFactory().Default();
            engine_playerEscape = new FuzzyEngineFactory().Default();

            distanceToPlayer = new LinguisticVariable("distanceToPlayer");
            var near = distanceToPlayer.MembershipFunctions.AddTrapezoid("Near", 0, 10, 25, 40);
            var mid = distanceToPlayer.MembershipFunctions.AddTrapezoid("Mid", 25, 40, 55, 70);
            var far = distanceToPlayer.MembershipFunctions.AddTrapezoid("Far", 55, 70, 85, 100);
            var veryfar = distanceToPlayer.MembershipFunctions.AddTrapezoid("VeryFar", 55, 90, 120, 160);

            chaseSpeed = new LinguisticVariable("chaseSpeed");
            var slow = chaseSpeed.MembershipFunctions.AddTrapezoid("Slow", 0.5, 0.7, 0.9, 1.2);
            var normal = chaseSpeed.MembershipFunctions.AddTrapezoid("Normal", 0.8, 1.4, 1.8, 2.2);
            var fast = chaseSpeed.MembershipFunctions.AddTrapezoid("Fast", 1.4, 2, 2.5, 4);

            lostPlayer = new LinguisticVariable("lostPlayer");
            var lost = lostPlayer.MembershipFunctions.AddRectangle("Lost", 0, 1);
            var notLost = lostPlayer.MembershipFunctions.AddRectangle("NotLost", -1, 0);

            var chaseSpeed_rule1 = Rule.If(distanceToPlayer.Is(far)).Then(chaseSpeed.Is(fast));
            var chaseSpeed_rule2 = Rule.If(distanceToPlayer.Is(mid)).Then(chaseSpeed.Is(normal));
            var chaseSpeed_rule3 = Rule.If(distanceToPlayer.Is(near)).Then(chaseSpeed.Is(slow));

            var lostPlayer_rule1 = Rule.If(distanceToPlayer.Is(veryfar)).Then(lostPlayer.Is(lost));
            var lostPlayer_rule2 = Rule.If(distanceToPlayer.Is(far).Or(distanceToPlayer.Is(mid)).Or(distanceToPlayer.Is(near))).Then(lostPlayer.Is(notLost));

            engine_chaseSpeed.Rules.Add(chaseSpeed_rule1, chaseSpeed_rule2, chaseSpeed_rule3);
            engine_playerEscape.Rules.Add(lostPlayer_rule1, lostPlayer_rule2);
        }

        public override void OnEnter()
        {
            shark.stateImage.color = Color.red;
            shark.animator.SetBool("Chasing", true);
        }

        public override Type Update()
        {
            if (shark.target == null) { return typeof(FuzzyRoamState); }

            chaseBoat();

            if (boatEscaped()) { return typeof(FuzzySearchState); }
            else { return typeof(FuzzyChaseState); }
        }

        void chaseBoat()
        {
            Debug.DrawRay(shark.transform.position, shark.target.position - shark.transform.position, Color.red);

            Quaternion targetRotation = Quaternion.LookRotation(shark.target.position - shark.transform.position); targetRotation.x = 0; targetRotation.z = 0;

            float playerDistance = Vector3.Distance(shark.transform.position, shark.target.position);

            double speed = engine_chaseSpeed.Defuzzify(new { distanceToPlayer = (double)playerDistance });

            shark.transform.rotation = Quaternion.Lerp(shark.transform.rotation, targetRotation, shark.turnSpeed * Time.deltaTime);
            shark.transform.position += (shark.transform.forward * (float)speed * shark.thrustSpeed) * Time.deltaTime;

            shark.animator.SetFloat("Speed", (float)speed);
        }

        bool boatEscaped()
        {
            float playerDistance = Vector3.Distance(shark.transform.position, shark.target.position);

            double lostPlayer = engine_playerEscape.Defuzzify(new { distanceToPlayer = (double)playerDistance });

            if (lostPlayer > 0)
                return true;
            else
                return false;
        }
    }
}


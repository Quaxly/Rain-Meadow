using System.Linq;
using UnityEngine;

namespace RainMeadow
{

    public class RealizedWeaponState : RealizedPhysicalObjectState
    {
        // is all this data really necessary?
        protected byte mode;
        private Vector2 tailPos;
        private Vector2? setRotation;
        private Vector2 rotation;
        private Vector2 lastRotation;
        private float rotationSpeed;
        
        public RealizedWeaponState() { }
        public RealizedWeaponState(OnlinePhysicalObject onlineEntity) : base(onlineEntity)
        {
            var weapon = (Weapon)onlineEntity.apo.realizedObject;
            mode = (byte)weapon.mode;
            tailPos = weapon.tailPos;
            setRotation = weapon.setRotation;
            rotation = weapon.rotation;
            lastRotation = weapon.lastRotation;
            rotationSpeed = weapon.rotationSpeed;
        }

        public override void ReadTo(OnlineEntity onlineEntity)
        {
            if (!onlineEntity.owner.isMe && onlineEntity.isPending) return; // Don't sync if pending, reduces visibility and effect of lag
            base.ReadTo(onlineEntity);
            var weapon = (Weapon)((OnlinePhysicalObject)onlineEntity).apo.realizedObject;
            var newMode = new Weapon.Mode(Weapon.Mode.values.GetEntry(mode));
            if (weapon.room != null && weapon.mode != newMode) weapon.ChangeMode(newMode);
            weapon.tailPos = tailPos;
            weapon.setRotation = setRotation;
            weapon.rotation = rotation;
            weapon.lastRotation = lastRotation;
            weapon.rotationSpeed = rotationSpeed;
        }
    }
}
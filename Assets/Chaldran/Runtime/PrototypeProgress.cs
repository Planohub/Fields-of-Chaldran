namespace Chaldran
{
    public sealed class PrototypeProgress
    {
        public bool HasWeapon { get; private set; }
        public bool WardenDefeated { get; private set; }
        public bool HasWriteAccess { get; private set; }
        public bool GateOpen { get; private set; }
        public bool Complete { get; private set; }

        public string Objective => !HasWeapon ? "Recover the weapon from the cyan anomaly."
            : !WardenDefeated ? "Defeat the clockwork sentinel."
            : !HasWriteAccess && !GateOpen ? "Collect the sentinel's Write-Access token."
            : !GateOpen ? "Override the red barrier to the northeast."
            : !Complete ? "Activate the exit terminal beyond the barrier."
            : "Quarantine trial complete.";

        public bool RecoverWeapon()
        {
            if (HasWeapon) return false;
            HasWeapon = true;
            return true;
        }

        public bool DefeatWarden()
        {
            if (!HasWeapon || WardenDefeated) return false;
            WardenDefeated = true;
            return true;
        }

        public bool CollectWriteAccess()
        {
            if (!WardenDefeated || HasWriteAccess || GateOpen) return false;
            HasWriteAccess = true;
            return true;
        }

        public bool OpenGate()
        {
            if (!HasWriteAccess || GateOpen) return false;
            HasWriteAccess = false;
            GateOpen = true;
            return true;
        }

        public bool Finish()
        {
            if (!GateOpen || Complete) return false;
            Complete = true;
            return true;
        }

        public void RestoreCompletedTutorial()
        {
            HasWeapon = true;
            WardenDefeated = true;
            HasWriteAccess = false;
            GateOpen = true;
            Complete = true;
        }
    }
}

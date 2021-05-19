namespace ss
{
    public sealed class Sword : Weapon
    {
        protected override void OnEquipped()
        {
            base.OnEquipped();

            var gameManager = GameManager.Instance;

            if (gameManager != null)
            {
                AudioManager.Instance.CreateTemporaryAudioSourceAt("SwordEquip", transform.position);
            }
        }
    }
}

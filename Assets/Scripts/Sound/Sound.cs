public enum Sound
{
    OnCardDragBegin = 0,
    OnDropCard = 1,
    OnButtonSelect = 2,
    OnButtonClicked = 3,
    OnPanelOpened = 4,
    PlayerWin = 5,
    Music1 = 6,
    Music2 = 7,
    Music3 = 8,
    CardDeal = 9,
    CardDiscard = 10,
    Ambience = 11,
    CritError = 12,
    FieldEffect = 13
}

public static class SoundExtensions
{
    public static void PlaySound(this Sound sound) =>
        SoundPlayer.Instance.PlayOnce(sound);
}

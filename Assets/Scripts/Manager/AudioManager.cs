using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("1. Nhạc nền (Background Music)")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float bgmVolume = 0.5f;

    [Header("2. Âm thanh Kéo Thả Gạch (DraggablePiece)")]
    public AudioClip pickupSound;           // Khi chạm vào nhấc gạch lên
    public AudioClip placeSound;            // Khi đặt gạch xuống bàn cờ thành công
    public AudioClip invalidMoveSound;      // Khi thả gạch sai vị trí (bị trả về khay)

    [Header("3. Âm thanh Kỹ năng Búa & Xu (BoosterManager)")]
    public AudioClip hammerSelectSound;     // Khi bấm nút Búa để bật chế độ chọn ô
    public AudioClip hammerCancelSound;     // Khi bấm nút Búa lần nữa để hủy chọn
    public AudioClip notEnoughCoinsSound;   // Khi bấm nút Búa nhưng không đủ Xu
    public AudioClip hammerSmashSound;      // Khi đập Búa nổ vùng 3x3 trên bàn cờ
    public AudioClip coinRewardSound;       // Khi được cộng thêm Xu lúc ăn hàng

    [Header("4. Âm thanh Ăn hàng & Chuỗi Combo (GameManager)")]
    public AudioClip clearLineSound;        // Phá hàng bình thường (1x)
    public AudioClip combo2xSound;          // Đạt Combo 2x
    public AudioClip combo3xSound;          // Đạt Combo 3x
    public AudioClip combo4xSound;          // Đạt Combo 4x
    public AudioClip combo5xSound;          // Đạt Combo 5x
    public AudioClip combo6xSound;          // Đạt Combo 6x
    public AudioClip combo7xPlusSound;      // Đạt Combo từ 7x trở đi (7x, 8x, 9x...)

    [Header("5. Âm thanh Giao diện & Trạng thái (UI & GameState)")]
    public AudioClip buttonClickSound;      // Nút bấm UI chung
    public AudioClip retryButtonSound;      // Khi bấm nút Retry (Chơi lại)
    public AudioClip newHighScoreSound;     // Khi phá kỷ lục điểm cao nhất (High Score)
    public AudioClip gameOverSound;         // Khi hiện bảng thua cuộc (Game Over)

    [Header("6. Cấu hình Âm lượng SFX chung")]
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    private void Start()
    {
        if (backgroundMusic != null)
        {
            bgmSource.clip = backgroundMusic;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlayPickup() => PlaySFX(pickupSound);
    public void PlayPlace() => PlaySFX(placeSound);
    public void PlayInvalidMove() => PlaySFX(invalidMoveSound);

    public void PlayHammerSelect() => PlaySFX(hammerSelectSound != null ? hammerSelectSound : buttonClickSound);
    public void PlayHammerCancel() => PlaySFX(hammerCancelSound != null ? hammerCancelSound : buttonClickSound);
    public void PlayNotEnoughCoins() => PlaySFX(notEnoughCoinsSound != null ? notEnoughCoinsSound : invalidMoveSound);
    public void PlayHammer() => PlaySFX(hammerSmashSound);
    public void PlayCoinReward() => PlaySFX(coinRewardSound);

    public void PlayClick() => PlaySFX(buttonClickSound);
    public void PlayRetry() => PlaySFX(retryButtonSound != null ? retryButtonSound : buttonClickSound);
    public void PlayNewHighScore() => PlaySFX(newHighScoreSound);
    public void PlayGameOver() => PlaySFX(gameOverSound);


    public void PlayCombo(int comboCount)
    {
        AudioClip clipToPlay = null;

        switch (comboCount)
        {
            case 2: clipToPlay = combo2xSound; break;
            case 3: clipToPlay = combo3xSound; break;
            case 4: clipToPlay = combo4xSound; break;
            case 5: clipToPlay = combo5xSound; break;
            case 6: clipToPlay = combo6xSound; break;
            default:
                clipToPlay = (comboCount >= 7) ? combo7xPlusSound : clearLineSound;
                break;
        }

        if (clipToPlay == null)
        {
            clipToPlay = clearLineSound;
        }

        PlaySFX(clipToPlay);
    }
}
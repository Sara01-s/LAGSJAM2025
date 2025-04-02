using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BetterButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler {
	public UnityEvent OnClick;
	public UnityEvent OnHover;

	[Header("Sounds")]
	[SerializeField] private AudioClip _clickSound;
	[SerializeField] private AudioClip _hoverSound;

	private IAudioService _audioService;

	private void Start() {
		_audioService = Services.Instance.GetService<IAudioService>();
		Assert.IsNotNull(_clickSound, "Click sound is not assigned in the inspector.");
		Assert.IsNotNull(_hoverSound, "Hover sound is not assigned in the inspector.");
	}

	public void OnPointerDown(PointerEventData eventData) {
		OnClick?.Invoke();
		_audioService.PlaySound(_clickSound);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		OnHover?.Invoke();
		_audioService.PlaySound(_hoverSound);
	}
}

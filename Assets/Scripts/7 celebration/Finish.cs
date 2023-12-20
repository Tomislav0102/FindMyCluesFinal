using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MileStone, IActivation
{
    [SerializeField] ParticleSystem[] psActivation;

    [SerializeField] Transform parFireworks;
    ParticleSystem[] _psFireworks;
    float _timerFireworks = Mathf.Infinity;
    int _counterFireworks;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if (_oneHitActivation) return;
            if (_isActive)
            {
                for (int i = 0; i < psActivation.Length; i++)
                {
                    psActivation[i].Play();
                    gm.characterTalking.CharacterInfoText("Congratulations, you have reached the end!", Emotion.Smile);
                }
            }
            _oneHitActivation = true;
        }
    }
    bool _isActive;
    bool _oneHitActivation;

    protected override void Awake()
    {
        base.Awake();
        _psFireworks = Utilities.GetallChildren<ParticleSystem>(parFireworks);
        ExperienceManager.MinMaxDistance = new Vector2(6f, Mathf.Infinity);
        gm.arFloor.SetActive(false);
    }
    public override void StartMe()
    {
        base.StartMe();
        IsActive = true;
    }

    private void Update()
    {
        if (!IsActive) return;

        _timerFireworks += Time.deltaTime;
        if (_timerFireworks > 1f)
        {
            _timerFireworks = 0f;
            _psFireworks[_counterFireworks].transform.localPosition = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
            _psFireworks[_counterFireworks].Play();
            _counterFireworks = (1 + _counterFireworks) % _psFireworks.Length;
        }
    }
}

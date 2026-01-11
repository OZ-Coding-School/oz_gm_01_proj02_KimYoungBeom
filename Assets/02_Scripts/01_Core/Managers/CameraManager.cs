using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // 0:Intro, 1:Quarter, 2:Top, 3:FirstPerson ,4:Lobby
    [SerializeField] private CinemachineCamera[] _vCams;
    [SerializeField] private CinemachineBrain _brain;

    [Header("감도 설정")]
    [SerializeField] private float _sensitivityMultiplier = 0.1f;
    [SerializeField] private float _maxSmoothTime = 0.2f;
    [SerializeField] private float _minSmoothTime = 0.03f;

    private readonly Dictionary<EViewMode, CinemachineCamera> _vCamsDic = new();
    private EViewMode _currentViewMode;
    private CancellationTokenSource _introCts;

    //감도설정
    private CinemachinePanTilt _panTilt;
    private float _currentPan, _currentTilt;
    private float _panVel, _tiltVel;
    private float _targetPan, _targetTilt;
    private float _currentSensitivity;

    #region Life Time
    private void Awake()
    {
        InitDictionary();
        ChangeView(EViewMode.Lobby);
        _panTilt = _vCamsDic[EViewMode.FirstPerson].GetComponent<CinemachinePanTilt>();
    }
    private void Update()
    {
        ApplyLinearSmoothing();
    }
    private void OnEnable()
    {
        Managers.Input.onLookEvent += OnLook;
        Managers.Data.onCamSensitivityChange += HandleSensitivityChange;
    }
    private void OnDisable()
    {
        Managers.Input.onLookEvent -= OnLook;
        Managers.Data.onCamSensitivityChange -= HandleSensitivityChange;
    }
    private void InitDictionary()
    {
        if (_vCams == null || _vCams.Length < 5) return;

        _vCamsDic.Clear();
        _vCamsDic.Add(EViewMode.Intro, _vCams[0]);
        _vCamsDic.Add(EViewMode.Quarter, _vCams[1]);
        _vCamsDic.Add(EViewMode.Top, _vCams[2]);
        _vCamsDic.Add(EViewMode.FirstPerson, _vCams[3]);
        _vCamsDic.Add(EViewMode.Lobby, _vCams[4]);
    }
    #endregion

    #region Event Handle
    private void OnLook(Vector2 delta)
    {
        if (_currentViewMode != EViewMode.FirstPerson) return;

        _targetPan += delta.x * _currentSensitivity;
        _targetTilt -= delta.y * _currentSensitivity;
    }
    private void ApplyLinearSmoothing()
    {
        if (_currentViewMode != EViewMode.FirstPerson) return;

        float t = Mathf.InverseLerp(Defines.CAM_SENS_MIN, Defines.CAM_SENS_MAX, _currentSensitivity);

        float currentSmoothTime = Mathf.Lerp(_maxSmoothTime, _minSmoothTime, t);

        _currentPan = Mathf.SmoothDampAngle(_currentPan, _targetPan, ref _panVel, currentSmoothTime);
        _currentTilt = Mathf.SmoothDamp(_currentTilt, _targetTilt, ref _tiltVel, currentSmoothTime);

        if (_panTilt != null)
        {
            _panTilt.TiltAxis.Value = _currentTilt;
            _panTilt.PanAxis.Value = _currentPan;
        }
    }
    private void HandleSensitivityChange()
    {
        _currentSensitivity = Managers.Data.CamSensitivity * _sensitivityMultiplier;
    }
    #endregion

    #region 외부호출 함수
    public float GetPanValue() => _currentPan;
    public void SetPlayerTarget(PlayerController player)
    {
        Transform eyePoint = player.EyePoint;

        foreach (var kvp in _vCamsDic)
        {
            EViewMode mode = kvp.Key;
            CinemachineCamera vcam = kvp.Value;

            if (mode == EViewMode.Intro || mode == EViewMode.Lobby) continue;

            if (mode == EViewMode.FirstPerson)
            {
                vcam.Follow = player.EyePoint;
            }
            else
            {
                vcam.Follow = player.transform;
                vcam.LookAt = player.transform;
            }
        }
    }

    public void ChangeView(EViewMode mode)
    {
        if (!_vCamsDic.ContainsKey(mode)) return;
        if (mode != EViewMode.Intro) CancelIntro();

        foreach (var kvp in _vCamsDic)
        {
            CinemachineCamera vcam = kvp.Value;
            bool isTarget = (kvp.Key == mode);
            vcam.Priority = isTarget ? 10 : 0;
        }

        _currentViewMode = mode;
    }
    #endregion

    #region 인트로 연출
    public async Awaitable StartStageIntro(Vector3 startPos, Vector3 endPos, float duration)
    {
        ChangeView(EViewMode.Lobby);
        await Awaitable.NextFrameAsync(destroyCancellationToken);
        ChangeView(EViewMode.Intro);
        CancelIntro();
        _introCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

        CinemachineCamera introCam = _vCamsDic[EViewMode.Intro];

        float scanDuration = duration * 0.65f;
        float returnDuration = duration * 0.36f;
        Vector3 offset = new Vector3(-1.0f, 5.5f, -5.0f);

        float elapsed = 0f;
        try
        {
            while (elapsed < scanDuration)
            {
                float t = elapsed / scanDuration;
                float curveT = Mathf.SmoothStep(0, 1, t);

                Vector3 currentTarget = Vector3.Lerp(startPos, endPos, curveT);
                introCam.transform.position = currentTarget + offset;
                introCam.transform.LookAt(currentTarget);

                var lens = introCam.Lens;
                lens.Dutch = Mathf.Lerp(-10f, 10f, curveT);
                introCam.Lens = lens;

                elapsed += Time.deltaTime;
                await Awaitable.NextFrameAsync(_introCts.Token);
            }
            Quaternion startRotation = introCam.transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(40.0f, 0.0f, 0.0f);
            elapsed = 0f;
            while (elapsed < returnDuration)
            {
                float t = elapsed / returnDuration;
                float curveT = Mathf.SmoothStep(0, 1, t);

                Vector3 currentTarget = Vector3.Lerp(endPos, startPos, curveT);
                introCam.transform.position = currentTarget + offset;
                introCam.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, curveT);

                var lens = introCam.Lens;
                lens.Dutch = Mathf.Lerp(10f, 0f, curveT);
                introCam.Lens = lens;

                elapsed += Time.deltaTime;
                await Awaitable.NextFrameAsync(_introCts.Token);
            }

            ChangeView(EViewMode.Quarter);
        }
        catch (OperationCanceledException)
        {
            var lens = introCam.Lens;
            lens.Dutch = 0;
            introCam.Lens = lens;
            Utils.Log("인트로 연출 중단 및 초기화");
        }
    }
    public void CancelIntro()
    {
        if (_introCts != null)
        {
            _introCts.Cancel();
            _introCts.Dispose();
            _introCts = null;
        }
    }
    #endregion
}
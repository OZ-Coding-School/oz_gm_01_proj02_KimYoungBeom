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

    private readonly Dictionary<EViewMode, CinemachineCamera> _vCamsDic = new();
    private CancellationTokenSource _introCts;
    private void Awake()
    {
        InitDictionary();
        ChangeView(EViewMode.Lobby);
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
        if (mode == EViewMode.Lobby)
        {
            _vCamsDic[EViewMode.Lobby].ForceCameraPosition(Vector3.zero, Quaternion.identity);
            _brain.ActiveBlend = null;
        }
    }

    public async Awaitable StartStageIntro(Vector3 startPos, Vector3 endPos, float duration)
    {
        ChangeView(EViewMode.Intro);
        CancelIntro();
        _introCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

        CinemachineCamera introCam = _vCamsDic[EViewMode.Intro];

        float scanDuration = duration * 0.7f;
        float returnDuration = duration * 0.3f;
        Vector3 offset = new Vector3(-5.0f, 6.0f, -5.0f);

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

            elapsed = 0f;
            while (elapsed < returnDuration)
            {
                float t = elapsed / returnDuration;
                float curveT = t * t;

                Vector3 currentTarget = Vector3.Lerp(endPos, startPos, curveT);
                introCam.transform.position = currentTarget + offset;
                introCam.transform.LookAt(currentTarget);

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
}
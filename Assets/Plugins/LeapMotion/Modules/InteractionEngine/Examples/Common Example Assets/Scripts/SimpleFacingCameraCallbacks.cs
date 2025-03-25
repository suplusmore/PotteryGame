/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2020.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Leap.Unity.Examples {

  [AddComponentMenu("")]
  public class SimpleFacingCameraCallbacks : MonoBehaviour {

    public Transform toFaceCamera;
    public Camera cameraToFace;

    private bool _initialized = false;
    private bool _isFacingCamera = false;

    public UnityEvent OnBeginFacingCamera;
    public UnityEvent OnEndFacingCamera;
        //ÑÕÉ«ºÐ
        public GameObject box1;
        public GameObject box2;
        public GameObject box3;
        public GameObject box4;
        public GameObject box5;
        public GameObject box6;
        //ÎÆÀíºÐ
        public GameObject texbox1;
        public GameObject texbox2;
        public GameObject shuazi;
        public GameObject ui;
        void Start() {
      if (toFaceCamera != null) initialize();
    }

    private void initialize() {
      if(cameraToFace == null) { cameraToFace = Camera.main; }
      // Set "_isFacingCamera" to be whatever the current state ISN'T, so that we are
      // guaranteed to fire a UnityEvent on the first initialized Update().
      _isFacingCamera = !GetIsFacingCamera(toFaceCamera, cameraToFace);
      _initialized = true;
    }

    void Update() {
      if (toFaceCamera != null && !_initialized) {
        initialize();
      }
      if (!_initialized) return;

      if (GetIsFacingCamera(toFaceCamera, cameraToFace, _isFacingCamera ? 0.77F : 0.82F) != _isFacingCamera) {
        _isFacingCamera = !_isFacingCamera;

        if (_isFacingCamera) {
          OnBeginFacingCamera.Invoke();
        }
        else {
          OnEndFacingCamera.Invoke();
        }
      }
    }
        public void addcolorbox()
        {
            texbox1.SetActive(false);
            texbox2.SetActive(false);
            box1.SetActive(true);
            box2.SetActive(true);
            box3.SetActive(true);
            box4.SetActive(true);
            box5.SetActive(true);
            box6.SetActive(true);
            shuazi.SetActive(true);
            ui.SetActive(false);
        }

        public void addtexbox()
        {
            box1.SetActive(false);
            box2.SetActive(false);
            box3.SetActive(false);
            box4.SetActive(false);
            box5.SetActive(false);
            box6.SetActive(false);
            texbox1.SetActive(true);
            texbox2.SetActive(true);
            shuazi.SetActive(true);
            ui.SetActive(false);
        }

        public void quxiaobox()
        {
            box1.SetActive(false);
            box2.SetActive(false);
            box3.SetActive(false);
            box4.SetActive(false);
            box5.SetActive(false);
            box6.SetActive(false);
            texbox1.SetActive(false);
            texbox2.SetActive(false);
            shuazi.SetActive(false);
        }

    public static bool GetIsFacingCamera(Transform facingTransform, Camera camera, float minAllowedDotProduct = 0.8F) {
      return Vector3.Dot((camera.transform.position - facingTransform.position).normalized, facingTransform.forward) > minAllowedDotProduct;
    }

  }

}

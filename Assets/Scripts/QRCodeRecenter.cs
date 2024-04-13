using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;

public class QRCodeRecenter : MonoBehaviour
{
    [SerializeField]
    private ARSession session;
    [SerializeField]
    private XROrigin sessionOrigin;
    [SerializeField]
    private ARCameraManager cameraManager;
    [SerializeField]
    private List<Target> navigationTargetObjects = new List<Target>();

    private Texture2D cameraImageTexture;
    private IBarcodeReader reader = new BarcodeReader();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
           SetQrCodeRecenterTarget("Living Room");
        }
    }
    private void OnEnable()
    {
        cameraManager.frameReceived += OnCameraFrameReceived; 
    }

    private void OnDisable()
    {
        cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if(!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            return;
        }

        var conversionParams = new XRCpuImage.ConversionParams()
        {
            // Get entire image
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Downsample by 2
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

            // Choose RGBA format
            outputFormat = TextureFormat.RGBA32,

            // Flip across vertical Axis (mirror image)
            transformation = XRCpuImage.Transformation.MirrorY
        };

        // See how many bytes you need to store the final image
        int size = image.GetConvertedDataSize(conversionParams);

        // Allocate a buffer to store the image
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        // Extract image data
        image.Convert(conversionParams, buffer);

        // dispose XRCPUImage
        image.Dispose();

        // apply texture
        cameraImageTexture = new Texture2D(
            conversionParams.outputDimensions.x,
            conversionParams.outputDimensions.y,
            conversionParams.outputFormat,
            false);

        cameraImageTexture.LoadRawTextureData(buffer);
        cameraImageTexture.Apply();

        // done with tepm data now dispose
        buffer.Dispose();

        // detect and decode barcode inside bitmap
        var result = reader.Decode(cameraImageTexture.GetPixels32(), cameraImageTexture.width, cameraImageTexture.height);

        // do something with result
        if(result != null)
        {
            SetQrCodeRecenterTarget(result.Text);
        }
    }

    private void SetQrCodeRecenterTarget(string targetText)
    {
        Target currentTarget = navigationTargetObjects.Find(x  => x.Name.ToLower().Equals(targetText.ToLower()));
        if(currentTarget != null)
        {
            // reset position and rotation of AR session
            session.Reset();

            // add offset for recentering
            sessionOrigin.transform.position = currentTarget.positionObject.transform.position;
            sessionOrigin.transform.rotation = currentTarget.positionObject.transform.rotation;
        }
    }
} // class

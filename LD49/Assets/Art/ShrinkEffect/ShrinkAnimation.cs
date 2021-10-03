using UnityEngine;

public class ShrinkAnimation : MonoBehaviour {

    [Header("Input")]
    public bool charging = false;
    public Vector3 chargeHitPosition = Vector3.zero;
    public bool currentlyLarge = true;
    public float shrinkScaleFactor = 0.3f;
    
    [Header("Internal Tweaking")]
    public float chargeLossSpeedFactor = .2f;
    public float smoothenRandomRotationMin = 0.6f;
    public float smoothenRandomRotationMax = 0.9f;
    public float randomRotationYDegrees = 30;
    public float randomRotationXDegrees = 30;
    public float randomRotationZDegrees = 30;
    public float scaleForceFactor = .3f;
    public float scaleForceDamp = .3f;
    public float maxCharge = 1f;
    public float chargeHitPositionTimeout = 0.5f;
    public float restartBuildUpTimeout = 0.5f;
    public float outsideScaleFactorSmoothTime;
    public float outsideScaleFactorStandardSize = 0.97f;
    public float outsideScaleFactorAddSize = 0.01f;
    public float minChargeLimit = 1.5f;
    public float minActualCharge = 3f;
    public float amountIndicationScale = .3f;
    public float amountIndicationScaleWhenSmall = .3f;
    public float chargeWaveHeightSmallFactor = .5f;
    public float chargeWaveScaleSmallFactor = 1f;

    [Header("Shader Tweaking")]
    public float AmountDisplace = 0;
    public float ExtraGlitchScaleFactor = 0;
    public float GlitchScaleImpactFactor = 4;
    public Vector3 ChargePoint = Vector3.zero;
    public Vector3 ChargePointSmooth = Vector3.zero;
    public float ChargeContributionStart = 0;
    public float ChargeContributionEnd = 0;
    public float ChargeTransitionToSmoothHitPoint = 2;
    public float ChargeWaveScale = 0.1f;
    public float ChargeWaveSpeed = -26;
    public float ChargeWaveHeight = 0.05f;
    public float ChargeTransitionWaveSpeed = 2;
    public float ShrinkEndShift = 0;
    public float ChargeEffectBuildupSpeed = 200;
    public float ExtraHeightHitPointFalloffShape = 10;
    public float ExtraHeightHitPoint = 2;


    [Header("Animated, Don't touch")]
    private float animatedRandomRotation = 1;


    [Header("Renderers to adapt")]
    public Renderer outsideRenderer;
    public Renderer insideRenderer;

    // -- Animation state --
    private float charge = 0f;

    // Internal (Glitch Rotation)
    private Quaternion initialRotation;
    private Quaternion lastRotation;

    // Internal (Glitch Scale)
    private Vector3 currentScaleVelocity;
    private Vector3 lastLossyScale;

    // Internal (Charge Hit)
    private Vector3 smoothChargePoint;
    private float lastTimeChargePointContribution;
    private bool wasCharging;
    private float chargeContributionStartTime;

    // Preview
    private Vector3 initialScale;
    private float outsideScaleFactor;
    private float outsideScaleFactorSpeed;

    // Charge Indication
    private float extraScaleFactor = 1;

    /* Set the default values in the shaders */
    private void InitShaders() {
        // That way we can have multiple materials where we don't have to worry whether these are configured right...
        insideRenderer.material.SetFloat("_AmountDisplace", AmountDisplace);
        insideRenderer.material.SetFloat("_ExtraGlitchScaleFactor", ExtraGlitchScaleFactor);
        insideRenderer.material.SetFloat("_GlitchScaleImpactFactor", GlitchScaleImpactFactor);
        insideRenderer.material.SetVector("_ChargePoint", ChargePoint);
        insideRenderer.material.SetVector("_ChargePointSmooth", ChargePointSmooth);
        insideRenderer.material.SetFloat("_ChargeContributionStart", ChargeContributionStart);
        insideRenderer.material.SetFloat("_ChargeContributionEnd", ChargeContributionEnd);
        insideRenderer.material.SetFloat("_ChargeTransitionToSmoothHitPoint", ChargeTransitionToSmoothHitPoint);
        insideRenderer.material.SetFloat("_ChargeWaveScale", ChargeWaveScale);
        insideRenderer.material.SetFloat("_ChargeWaveSpeed", ChargeWaveSpeed);
        insideRenderer.material.SetFloat("_ChargeWaveHeight", ChargeWaveHeight);
        insideRenderer.material.SetFloat("_ChargeTransitionWaveSpeed", ChargeTransitionWaveSpeed);
        insideRenderer.material.SetFloat("_ShrinkEndShift", ShrinkEndShift);
        insideRenderer.material.SetFloat("_ChargeEffectBuildupSpeed", ChargeEffectBuildupSpeed);
        insideRenderer.material.SetFloat("_ExtraHeightHitPointFalloffShape", ExtraHeightHitPointFalloffShape);
        insideRenderer.material.SetFloat("_ExtraHeightHitPoint", ExtraHeightHitPoint);
    }

    private void Start() {
        // Reset the shaders
        InitShaders();

        // Init
        lastTimeChargePointContribution = Time.time - 100;
        chargeContributionStartTime = Time.time - 100;
        initialRotation = insideRenderer.transform.localRotation;
        lastLossyScale = insideRenderer.transform.lossyScale;

        initialScale = insideRenderer.transform.localScale;

        outsideScaleFactor = 1f;
    }

    private void SwitchToSmall() {
        // Throw an event or something? Do this elsewhere?
        currentlyLarge = false;
        insideRenderer.transform.localScale = initialScale * shrinkScaleFactor;
    }

    private void SwitchToLarge() {
        // Throw an event or something? Do this elsewhere?
        currentlyLarge = true;
        insideRenderer.transform.localScale = initialScale;
    }

    private void Update() {
        if(charging) {
            // We are charging!
            charge+=Time.deltaTime;

            // Restart the animation if it ended long enough ago
            if(!wasCharging && (Time.time-lastTimeChargePointContribution) > restartBuildUpTimeout) {
                chargeContributionStartTime = Time.time;
            }

            lastTimeChargePointContribution = Time.time;
        } else {
            charge-=Time.deltaTime * chargeLossSpeedFactor;
            if(charge < 0) {
                charge = 0;
                if(!currentlyLarge) {
                    SwitchToLarge();
                }
            }
        }
        if(currentlyLarge && charge > minChargeLimit && wasCharging) {
            // Give the user some more charge even if he charged very shortly
            charge = Mathf.Max(charge, minActualCharge);

            if(!charging) {
                // Ok, we stopped charging! Switch to small!
                SwitchToSmall();
            } else if(charge > maxCharge) {
                // Can not charge anymore... Switch to small!
                SwitchToSmall();
            }
        }

        if(charge > maxCharge) {
            charge = maxCharge;
        }

        // Show how much we are charged
        ShowCharged();

        // Expand Preview when needed
        ExpandPreview();

        // Glitch the rotation
        GlitchRotation();

        // Glitch when shrinking!
        ShrinkGlitch();

        // Animate charge point
        AnimateChargePoint();

        // Update state
        wasCharging = charging;
    }

    private void ShowCharged() {
        extraScaleFactor = 1;
        animatedRandomRotation = 0;
        if(currentlyLarge) {
            // Start glitching if we want to shrink
            float percentCharged = charge/maxCharge;

            // 
            animatedRandomRotation = percentCharged;
            extraScaleFactor = 1-percentCharged * amountIndicationScale;
        } else {
            // Small... Show when we will expand again!
            float percentUnCharged = 1-charge/maxCharge;
            extraScaleFactor = 1 + percentUnCharged * amountIndicationScaleWhenSmall;
            animatedRandomRotation = percentUnCharged;
        }
    }

    private void ExpandPreview() {
        float targetOutsideScaleFactor = outsideScaleFactorStandardSize;
        if(charging && currentlyLarge) {
            targetOutsideScaleFactor += outsideScaleFactorAddSize;
        }

        outsideScaleFactor = Mathf.SmoothDamp(outsideScaleFactor, targetOutsideScaleFactor, ref outsideScaleFactorSpeed, outsideScaleFactorSmoothTime);
        outsideRenderer.transform.localScale = initialScale * outsideScaleFactor;
    }

    private void AnimateChargePoint() {
        smoothChargePoint = Vector3.Lerp(smoothChargePoint, chargeHitPosition, Mathf.Clamp01((Time.time-lastTimeChargePointContribution)/chargeHitPositionTimeout + 0.1f));

        insideRenderer.material.SetVector("_ChargePoint", chargeHitPosition);
        insideRenderer.material.SetVector("_ChargePointSmooth", smoothChargePoint);
        insideRenderer.material.SetFloat("_ChargeContributionStart", -(chargeContributionStartTime - Time.time));
        insideRenderer.material.SetFloat("_ChargeContributionEnd", -(lastTimeChargePointContribution - Time.time));

        // Size of the charge effect depends on whether we are large or small
        insideRenderer.material.SetFloat("_ChargeWaveScale", ChargeWaveScale * (currentlyLarge?1:chargeWaveScaleSmallFactor));
        insideRenderer.material.SetFloat("_ChargeWaveHeight", ChargeWaveHeight * (currentlyLarge?1:chargeWaveHeightSmallFactor));
        insideRenderer.material.SetFloat("_ExtraHeightHitPoint", ExtraHeightHitPoint * (currentlyLarge?1:chargeWaveHeightSmallFactor));
    }

    private void GlitchRotation() {
        // Get a new random rotation!
        Quaternion newRandomRotationY = Quaternion.AngleAxis(Random.Range(-randomRotationYDegrees/2, randomRotationYDegrees/2), Vector3.up);
        Quaternion newRandomRotationX = Quaternion.AngleAxis(Random.Range(-randomRotationXDegrees/2, randomRotationXDegrees/2), Vector3.right);
        Quaternion newRandomRotationZ = Quaternion.AngleAxis(Random.Range(-randomRotationZDegrees/2, randomRotationZDegrees/2), Vector3.forward);
        Quaternion newRandomRotation = newRandomRotationY * newRandomRotationX * newRandomRotationZ;
        
        // Slerp them ?
        lastRotation = Quaternion.Slerp(lastRotation, newRandomRotation, 1-(animatedRandomRotation*(smoothenRandomRotationMin-smoothenRandomRotationMax)+smoothenRandomRotationMax));

        // Update rotation
        // How much should we apply them?
        Quaternion percentGlitchedRotation = Quaternion.Slerp(Quaternion.identity, lastRotation, animatedRandomRotation);
        insideRenderer.transform.localRotation = percentGlitchedRotation * initialRotation;
    }

    private void ShrinkGlitch() {
        Vector3 currentScale = insideRenderer.transform.lossyScale;

        // Interpolate the scale!
        currentScaleVelocity = currentScaleVelocity + (currentScale - lastLossyScale)*scaleForceFactor;
        currentScaleVelocity *= 1 - scaleForceDamp;

        // Tell the shader to glitch the scale transition
        lastLossyScale += currentScaleVelocity;
        insideRenderer.material.SetFloat("_ExtraGlitchScaleFactor", lastLossyScale.x/currentScale.x*extraScaleFactor-1.0f);
    }
}
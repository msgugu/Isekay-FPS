using UnityEngine;

namespace InGame.UI.Tweens
{
    // 각 이징 함수는 애니메이션의 특정 시점에서 어떤 값으로 속성을 업데이트할지 결정하는 역할
	// 애니메이션에 다양한 동적 효과를 부여
    internal class TweenEasingHandler
	{
        /// <summary>
        /// 지정된 Easing Type 함수적용
        /// </summary>
        /// <param name="e"> 사용할 이징 타입 </param>
        /// <param name="t"> 경과시간(애니메이션이 시작한 이후부터 현재까지의 시간) </param>
        /// <param name="b"> 시작 값(애니메이션 대상 속성의 초기 값)</param>
        /// <param name="c"> 변화량(애니메이션이 적용될 속성의 시작 값과 목표 값 사이의 차이) </param>
        /// <param name="d"> 지속 시간(애니메이션이 완료되기까지 걸리는 총 시간) </param>
        public static float Apply(TweenEasing e, float t, float b, float c, float d)
		{
			switch (e)
			{

                // 기본적인 2차 방정식을 사용하여 끝에서 속도가 감소하는 모션을 생성
                case TweenEasing.Swing:
				{
					return -c *(t/=d)*(t-2f) + b;
				}

                // 속도가 점점 빨라지거나 느려지는 효과를 2차 방정식으로 구현
                case TweenEasing.InQuad:
				{
					return c*(t/=d)*t + b;
				}
				case TweenEasing.OutQuad:
				{
					return -c *(t/=d)*(t-2) + b;
				}
				case TweenEasing.InOutQuad:
				{
					if ((t/=d/2) < 1) return c/2*t*t + b;
					return -c/2 * ((--t)*(t-2) - 1) + b;
				}

                //  3차 방정식을 사용해 더욱 강조된 가속 또는 감속 효과
                case TweenEasing.InCubic:
				{
					return c*(t/=d)*t*t + b;
				}
				case TweenEasing.OutCubic:
				{
					return c*((t=t/d-1)*t*t + 1) + b;
				}
				case TweenEasing.InOutCubic:
				{
					if ((t/=d/2) < 1) return c/2*t*t*t + b;
					return c/2*((t-=2)*t*t + 2) + b;
				}

                // 4차 방정식을 사용해 가속과 감속을 더욱 강하게 표현
                case TweenEasing.InQuart:
				{
					return c*(t/=d)*t*t*t + b;
				}
				case TweenEasing.OutQuart:
				{
					return -c * ((t=t/d-1)*t*t*t - 1) + b;
				}
				case TweenEasing.InOutQuart:
				{
					if ((t/=d/2) < 1) return c/2*t*t*t*t + b;
					return -c/2 * ((t-=2)*t*t*t - 2) + b;
				}

                // 5차 방정식으로 더욱 강한 가속 및 감속 효과를 구현
                case TweenEasing.InQuint:
				{
					return c*(t/=d)*t*t*t*t + b;
				}
				case TweenEasing.OutQuint:
				{
					return c*((t=t/d-1)*t*t*t*t + 1) + b;
				}
				case TweenEasing.InOutQuint:
				{
					if ((t/=d/2) < 1) return c/2*t*t*t*t*t + b;
					return c/2*((t-=2)*t*t*t*t + 2) + b;
				}

                // 사인 곡선을 기반으로 부드러운 시작과 끝을 가지는 모션을 생성
                case TweenEasing.InSine:
				{
					return -c * Mathf.Cos(t/d * (Mathf.PI/2)) + c + b;
				}
				case TweenEasing.OutSine:
				{
					return c * Mathf.Sin(t/d * (Mathf.PI/2)) + b;
				}
				case TweenEasing.InOutSine:
				{
					return -c/2 * (Mathf.Cos(Mathf.PI*t/d) - 1) + b;
				}

                // 지수 함수를 사용해 매우 빠른 가속 또는 감속을 표현
                case TweenEasing.InExpo:
				{
					return (t==0) ? b : c * Mathf.Pow(2, 10 * (t/d - 1)) + b;
				}
				case TweenEasing.OutExpo:
				{
					return (t==d) ? b+c : c * (-Mathf.Pow(2, -10 * t/d) + 1) + b;
				}
				case TweenEasing.InOutExpo:
				{
					if (t==0) return b;
					if (t==d) return b+c;
					if ((t/=d/2) < 1) return c/2 * Mathf.Pow(2, 10 * (t - 1)) + b;
					return c/2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;
				}

                //원형 함수를 사용해 독특한 가속 및 감속 효과를 만듬
                case TweenEasing.InCirc:
				{
					return -c * (Mathf.Sqrt(1 - (t/=d)*t) - 1) + b;
				}
				case TweenEasing.OutCirc:
				{
					return c * Mathf.Sqrt(1 - (t=t/d-1)*t) + b;
				}
				case TweenEasing.InOutCirc:
				{
					if ((t/=d/2) < 1) return -c/2 * (Mathf.Sqrt(1 - t*t) - 1) + b;
					return c/2 * (Mathf.Sqrt(1 - (t-=2)*t) + 1) + b;
				}

                // 초과 동작을 포함하여 뒤로 당겨지거나 앞으로 밀려나는 효과를 생성
                case TweenEasing.InBack:
				{
					float s = 1.70158f;
					return c*(t/=d)*t*((s+1f)*t - s) + b;
				}
				case TweenEasing.OutBack:
				{
					float s = 1.70158f;
					return c*((t=t/d-1f)*t*((s+1f)*t + s) + 1f) + b;
				}
				case TweenEasing.InOutBack:
				{
					float s = 1.70158f;
					if ((t/=d/2f) < 1f) return c/2f*(t*t*(((s*=(1.525f))+1f)*t - s)) + b;
					return c/2f*((t-=2f)*t*(((s*=(1.525f))+1f)*t + s) + 2f) + b;
				}

                // 튕기는 효과를 구현합니다. 매우 탄력적인 모션을 생성
                case TweenEasing.InBounce:
				{
					return c - TweenEasingHandler.Apply(TweenEasing.OutBounce, d-t, 0f, c, d) + b;
				}
				case TweenEasing.OutBounce:
				{
					if ((t/=d) < (1f/2.75f))
					{
						return c*(7.5625f*t*t) + b;
					}
					else if (t < (2f/2.75f))
					{
						return c*(7.5625f*(t-=(1.5f/2.75f))*t + .75f) + b;
					}
					else if (t < (2.5f/2.75f))
					{
						return c*(7.5625f*(t-=(2.25f/2.75f))*t + .9375f) + b;
					}
					else
					{
						return c*(7.5625f*(t-=(2.625f/2.75f))*t + .984375f) + b;
					}
				}
				case TweenEasing.InOutBounce:
				{
					if (t < d/2f) return TweenEasingHandler.Apply(TweenEasing.InBounce, t*2f, 0f, c, d) * .5f + b;
					return TweenEasingHandler.Apply(TweenEasing.OutBounce, t*2f-d, 0f, c, d) * .5f + c*.5f + b;
				}

                //고무 밴드처럼 늘어났다가 줄어드는 탄성 있는 애니메이션을 만듬
                case TweenEasing.InElastic:
				{
					float s=1.70158f; float p=0f; float a=c;
					if (t==0f) return b;
					if ((t/=d)==1f) return b+c;
					if (p==0f) p=d*.3f;
					if (a < Mathf.Abs(c)) { a=c; s=p/4f; }
					else s = p/(2f*Mathf.PI) * Mathf.Asin(c/a);
					if (float.IsNaN(s)) s = 0f;
					return -(a*Mathf.Pow(2f,10f*(t-=1f)) * Mathf.Sin((t*d-s)*(2f*Mathf.PI)/p )) + b;
				}
				case TweenEasing.OutElastic:
				{
					float s=1.70158f; float p=0f; float a=c;
					if (t==0f) return b; if ((t/=d)==1f) return b+c; if (p==0f) p=d*.3f;
					if (a < Mathf.Abs(c)) { a=c; s=p/4f; }
					else s = p/(2f*Mathf.PI) * Mathf.Asin(c/a);
					if (float.IsNaN(s)) s = 0f;
					return a*Mathf.Pow(2f,-10f*t) * Mathf.Sin((t*d-s)*(2f*Mathf.PI)/p ) + c + b;
				}
				case TweenEasing.InOutElastic:
				{
					float s=1.70158f; float p=0f; float a=c;
					if (t==0f) return b; if ((t/=d/2f)==2f) return b+c; if (p==0f) p=d*(.3f*1.5f);
					if (a < Mathf.Abs(c)) { a=c; s=p/4f; }
					else s = p/(2f*Mathf.PI) * Mathf.Asin(c/a);
					if (float.IsNaN(s)) s = 0f;
					if (t < 1f) return -.5f*(a*Mathf.Pow(2f,10f*(t-=1f)) * Mathf.Sin((t*d-s)*(2f*Mathf.PI)/p )) + b;
					return a*Mathf.Pow(2f,-10f*(t-=1f)) * Mathf.Sin((t*d-s)*(2f*Mathf.PI)/p )*.5f + c + b;
				}

                // 선형 이징 함수(애니메이션이 균일한 속도로 진행)
                case TweenEasing.Linear:
				default:
				{
					return c*t/d + b;
				}
			}
		}
	}
}

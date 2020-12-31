using UnityEngine;
using UnityEngine.UI;

namespace ViewToUnity {
    public class BarGraph : MonoBehaviour {

        [SerializeField] private Slider slider;
        [SerializeField] private Image background;
        [SerializeField] private Image fill;

        public Color BackgroundColor {
            set {
                if ( background != null )
                    background.color = value;
            }
        }

        public Color FillColor {
            set {
                if ( fill != null )
                    fill.color = value;
            }
        }
        private void Awake( )
            {
                if ( slider == null ) {
                    slider = gameObject.GetComponent<Slider>( );
                }
            }

        public void SetValue( float val )
            {
                if ( slider != null ) {
                    if ( slider.wholeNumbers ) {
                        Debug.Log( "Setting to float numbers" );
                        slider.wholeNumbers = false;
                    }
                    slider.value = val;
                }
            }

        public void SetValue( int val )
            {
                if ( slider != null ) {
                    if ( !slider.wholeNumbers ) {
                        Debug.Log( "Setting to whole numbers" );
                        slider.wholeNumbers = true;
                    }
                    
                    slider.value = val;
                }
            }

        public void SetMax( int value )
            {
                if ( slider != null )
                    slider.maxValue = value;
            }
        
        public void SetMin( int value )
            {
                if ( slider != null )
                    slider.minValue = value;
            }

    }
}
package {
	import flash.display.Sprite;
	import flash.text.*;
	
	public class TestASApp extends Sprite
	{
		public function TestASApp()
		{
			var tf:TextField = new TextField();
			tf.text = "test text: " + Version;
			
			addChild(tf);
		}
	}
}

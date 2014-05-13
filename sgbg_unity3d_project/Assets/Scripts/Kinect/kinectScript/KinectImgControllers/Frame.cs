
using System.Collections;

public class Frame {

	private short[] frame;

	public Frame(short[] data){
		frame = new short[data.Length];

		for (int i = 0; i < data.Length; i++) {
			frame[i] = data[i];
				}
		}

	public short[] getFrame(){
		return frame;
	}
}

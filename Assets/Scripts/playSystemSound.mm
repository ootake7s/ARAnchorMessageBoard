#import <Foundation/Foundation.h>
#import <AudioToolBox/AudioToolBox.h>

extern "C" void playSystemSound(int n)
{
    AudioServicesPlaySystemSound(n);
}

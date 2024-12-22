//
//  DiskSpaceHandler.mm
//

#import <Foundation/Foundation.h>
#import <sys/mount.h>

extern "C" {
	long long _GetAvailableFreeSpace(const char *path) {
		 struct statfs tStats;
         statfs("/", &tStats);
         return (long long)tStats.f_bavail * tStats.f_bsize;
	}	
}
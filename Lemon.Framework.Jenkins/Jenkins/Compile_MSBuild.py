#coding:utf-8
import os
import sys
import subprocess
import time
from pathlib import Path
import subprocess
sys.path.append(os.path.abspath(os.path.join(os.path.realpath(__file__), "../")))

CUR_PATH = os.getcwd()
PROJ_PATH = os.path.join(CUR_PATH, "../")
MSBUILD_EXE = '\"C:/Program Files/JetBrains/JetBrains Rider 2022.2.4/tools/MSBuild/Current/Bin/MSBuild.exe\"'
SLN_PATH = '\"E:/Projects/Lemon/Lemon.Framework.Jenkins/Lemon.Framework.Jenkins.sln\"'
BRANCH = 'gl-master'

isSuc = True
compile_output_list = []

def git_reset_pull():
	os.chdir(PROJ_PATH)
	cmd = 'git fetch --all' #git 拉取命令
	result = os.system(cmd)	
	cmd = 'git reset --hard HEAD' #git reset命令
	result = os.system(cmd)
	cmd = r"{0}{1}".format("git checkout ",BRANCH)
	result = os.system(cmd)
	cmd = 'git clean -fd' #git clean 命令
	result = os.system(cmd)
	cmd = 'git pull --rebase' #git pull命令
	result = os.system(cmd)

	if result == 0:
		print('git update succes')
	else:
		print('git update fail')

# 调用unity中我们封装的静态函数
def ms_build():
    cmd = '%s %s /t:Build /p:Configuration=Debug /p:Platform="Any CPU"'%(MSBUILD_EXE,SLN_PATH)
    print('run cmd:  ' + cmd)
    #os.system(cmd)
    # 运行命令并捕获输出
    process = subprocess.run(cmd, capture_output=True, text=True, shell=True)

    # 打印标准输出和标准错误
    #print("Standard Output:", process.stdout)
    #print("Standard Error:", process.stderr)

    # 检查命令返回代码
    global isSuc
    if process.returncode != 0:
        isSuc = False
        print("[Command] Scripts have compiler errors:", process.returncode)        
    else:
        isSuc = True
        print("[Command] CSharp Compiler successfully")
 
if __name__ == '__main__':	
	now = time.time() 
	git_reset_pull()
	ms_build()
	#error_file
	print(f'isSuc {isSuc}')
	print(f'total take time {time.time()-now} seconds')
	if not isSuc:
		sys.exit(1)
	else:	
		print("Done!")
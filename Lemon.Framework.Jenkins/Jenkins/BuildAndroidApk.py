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
BRANCH = 'gl-master'

# Unity的执行路径
unity_path = "C:/Program Files/Unity/Editor/Unity.exe"
# Unity项目的路径
project_path = "E:/Projects/Lemon/Lemon.Framework.Jenkins"
# 要执行的Unity编辑器自定义方法的名称，这个方法在Unity编辑器扩展脚本中定义
method_name = "Jenkins.BuildScript.BuildForAndroid"
# 打包后的APK文件路径
apk_output_path = "E:/Projects/Lemon/Lemon.Framework.Jenkins/Jenkins.apk"

# 拼接Unity命令行
cmd = [
    unity_path,
    "-quit",  # 表示Unity完成命令后关闭
    "-batchmode",  # 不显示界面和对话框
    "-nographics",  # 在支持的平台上不初始化图形设备
    "-silent-crashes",  # 自动处理崩溃情况
    "-projectPath", project_path,
    "-executeMethod", method_name,
    "-logFile",  # 可以指定日志文件路径，例如"-logFile", "unity.log"
    "-buildOutput", apk_output_path,
]

# 杀掉unity进程
def kill_unity():
    os.system('taskkill /IM Unity.exe /F')

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
def build():    
	# 执行命令行
	# subprocess.call(cmd) 注释掉这行，并替换为下面的代码，以阻塞直到命令完成并捕获输出
	process = subprocess.Popen(cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
	stdout, stderr = process.communicate()

	if process.returncode == 0:
		print("Build succeeded")
		print(stdout.decode("utf-8"))  # 显示标准输出
	else:
		print("Build failed")
		print(stderr.decode("utf-8"))  # 显示错误输出
		sys.exit(1)
 
if __name__ == '__main__':	
	now = time.time() 
	kill_unity()
	#git_reset_pull()
	build()
	print(f'total take time {time.time()-now} seconds')
	print("Done!")
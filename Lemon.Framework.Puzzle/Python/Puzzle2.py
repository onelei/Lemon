import cv2
import numpy as np

# 读取美术效果图和带有alpha通道的PNG图片
effect_image = cv2.imread('23.png')
png_image = cv2.imread('22.png', cv2.IMREAD_UNCHANGED)

# 提取PNG图片中的alpha通道并创建一个掩码
alpha_channel = png_image[:, :, 3]
_, mask = cv2.threshold(alpha_channel, 1, 255, cv2.THRESH_BINARY)

# 将PNG图片的RGB通道与掩码结合
png_rgb = png_image[:, :, :3]
png_rgb = cv2.bitwise_and(png_rgb, png_rgb, mask=mask)

# 在美术效果图中寻找匹配区域
result = cv2.matchTemplate(effect_image, png_rgb, cv2.TM_CCOEFF_NORMED, mask=mask)
min_val, max_val, min_loc, max_loc = cv2.minMaxLoc(result)

# 返回匹配的位置坐标
top_left = max_loc
bottom_right = (top_left[0] + png_rgb.shape[1], top_left[1] + png_rgb.shape[0])

print(f'Top-left corner: {top_left}')
print(f'Bottom-right corner: {bottom_right}')

# 可视化匹配区域
cv2.rectangle(effect_image, top_left, bottom_right, (0, 255, 0), 2)
cv2.imshow('Matched Image', effect_image)
cv2.waitKey(0)
cv2.destroyAllWindows()
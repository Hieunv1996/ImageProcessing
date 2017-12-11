# Image Processing

Project môn xử lí ảnh

# Làm sao để lấy màu R, G, B , A từ mảng một chiều(lấy từ bitmapData.Scan0) - tạm gọi là pixelArr ???

NOTE: Ma trận pixelArr duyệt màu từ ảnh theo hướng từ trên xuống , từ trái sang. (Xem file excel đi kèm)
```
	Tạm gọi ảnh là image.
	ma trận pixel ảnh là pixel
	Các pixel của ảnh sẽ bắt đầu từ pixel[0][0] tới pixel[width - 1][height - 1]
```
- Ảnh Gray (có Bit Depth = 8) => R = G = B, pixelArr chỉ cần 1 ô nhớ để lưu giá trị màu cho mỗi pixel 

- Ảnh RGB (có Bit Depth = 24) => R = G = B, pixelArr cần 3 ô nhớ liên tiếp để lưu giá trị màu cho mỗi pixel

- Ảnh RGBA (có Bit Depth = 32) => R = G = B, pixelArr chỉ cần 4 ô nhớ liên tiếp để lưu giá trị màu cho mỗi pixel

- Phương thức BitMapData.Stride trả về giá trị = Image.Width * BitDepth/8(số bytes được dùng cho biểu diễn màu).
```
Cụ thể:
	BitMapData.Stride : Chính là số ô nhớ liên tiếp cần dùng để biểu diễn giá trị màu 1 hàng của ảnh
	VD: Ảnh có kích thước 500 x 600. Bit Depth = 24. => BitMapData.Stride = 500 * (24/3).
		Vậy hàng đầu tiên của ảnh sẽ bắt đầu từ pixelArr[0] -> pixelArr[BitMapData.Stride - 1]. 
			pixelArr[0], pixelArr[1], pixelArr[2] lần lượt biểu diễn màu R, G, B của pixel[0][0] của ảnh,
			pixelArr[3], pixelArr[4], pixelArr[5] lần lượt biểu diễn màu R, G, B của pixel[0][1] của ảnh,
			pixelArr[BitMapData.Stride - 3], pixelArr[BitMapData.Stride - 2], pixelArr[BitMapData.Stride - 1] lần lượt biểu diễn màu R, G, B của pixel[0][image.Width - 1] của ảnh
		Hàng thứ 2 của ảnh bắt đầu từ pixelArr[BitMapData.Stride]
		...
```

- Kết luận:
```
Vậy để lấy giá trị màu của ảnh tại (x,y) ta cần:
- Input: + Bit Depth của ảnh
		 + Width, Height của ảnh
		 + Mảng pixelArr
- Output: Chỉ số bắt đầu của mảng pixelArr lưu giá trị màu của pixel tại tọa độ x, y.
- Quy trình:
	Xem file excel
```
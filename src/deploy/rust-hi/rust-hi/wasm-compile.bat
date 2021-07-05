rustup target add wasm32-wasi
rustc src/main.rs --target wasm32-wasi -o wasm-rust-hi.wasm
xcopy wasm-rust-hi.wasm ..\..\wasm-rust-hi\
del wasm-rust-hi.wasm